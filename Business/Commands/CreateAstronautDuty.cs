using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using StargateAPI.Business.Enums;
using StargateAPI.Business.Helpers;
using StargateAPI.Business.Services;
using System.Net;
using System.Text.Json;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required int RankId { get; set; }

        public required int DutyTitleId { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context;
        private readonly ILoggingService _loggingService;

        public CreateAstronautDutyPreProcessor(StargateContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }

        public async Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var requestData = JsonSerializer.Serialize(new { request.Name, request.RankId, request.DutyTitleId, request.DutyStartDate });
            await _loggingService.LogInfoAsync($"Starting astronaut duty creation validation for person: {request.Name}", "CreateAstronautDutyPreProcessor", "Process", requestData);

            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person is null) 
            {
                await _loggingService.LogWarningAsync($"Person '{request.Name}' not found during astronaut duty creation", "CreateAstronautDutyPreProcessor", "Process", requestData);
                throw new BadHttpRequestException($"Person '{request.Name}' not found. Please create the person first before assigning astronaut duties.", 404);
            }

            // Validate that the rank exists
            var rank = _context.Ranks.AsNoTracking().FirstOrDefault(r => r.Id == request.RankId && r.IsActive);
            if (rank is null)
            {
                await _loggingService.LogWarningAsync($"Invalid rank ID '{request.RankId}' provided for person '{request.Name}'", "CreateAstronautDutyPreProcessor", "Process", requestData);
                throw new BadHttpRequestException($"Rank with ID '{request.RankId}' not found or is inactive. Please use a valid rank ID.", 400);
            }

            // Validate that the duty title exists
            var dutyTitle = _context.DutyTitles.AsNoTracking().FirstOrDefault(d => d.Id == request.DutyTitleId && d.IsActive);
            if (dutyTitle is null)
            {
                await _loggingService.LogWarningAsync($"Invalid duty title ID '{request.DutyTitleId}' provided for person '{request.Name}'", "CreateAstronautDutyPreProcessor", "Process", requestData);
                throw new BadHttpRequestException($"Duty title with ID '{request.DutyTitleId}' not found or is inactive. Please use a valid duty title ID.", 400);
            }

            var verifyNoPreviousDuty = _context.AstronautDuties
                .Include(ad => ad.DutyTitle)
                .FirstOrDefault(z => z.DutyTitleId == request.DutyTitleId && z.DutyStartDate == request.DutyStartDate);

            if (verifyNoPreviousDuty is not null) 
            {
                await _loggingService.LogWarningAsync($"Duplicate duty attempt for person '{request.Name}' with duty title ID '{request.DutyTitleId}' and start date '{request.DutyStartDate:yyyy-MM-dd}'", "CreateAstronautDutyPreProcessor", "Process", requestData);
                throw new BadHttpRequestException($"An astronaut duty with duty title ID '{request.DutyTitleId}' and start date '{request.DutyStartDate:yyyy-MM-dd}' already exists.", 400);
            }

            await _loggingService.LogInfoAsync($"Validation successful for astronaut duty creation for person: {request.Name}", "CreateAstronautDutyPreProcessor", "Process", requestData);
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;
        private readonly ILoggingService _loggingService;

        public CreateAstronautDutyHandler(StargateContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var requestData = JsonSerializer.Serialize(new { request.Name, request.RankId, request.DutyTitleId, request.DutyStartDate });
            await _loggingService.LogInfoAsync($"Starting astronaut duty creation for person: {request.Name}", "CreateAstronautDutyHandler", "Handle", requestData);

            try
            {
                // Get person
                var person = await _context.People.FirstOrDefaultAsync(p => p.Name == request.Name);

                // Get rank and duty title
                var rank = await _context.Ranks.FirstOrDefaultAsync(r => r.Id == request.RankId);
                var dutyTitle = await _context.DutyTitles.FirstOrDefaultAsync(d => d.Id == request.DutyTitleId);

                // Get or create astronaut detail
                var astronautDetail = await _context.AstronautDetails.FirstOrDefaultAsync(ad => ad.PersonId == person.Id);

                if (astronautDetail == null)
                {
                    astronautDetail = new AstronautDetail
                    {
                        PersonId = person.Id,
                        CareerStartDate = request.DutyStartDate.Date,
                        CareerEndDate = EnumHelper.IsRetired(dutyTitle.Title) ? request.DutyStartDate.AddDays(-1).Date : null
                    };
                    await _context.AstronautDetails.AddAsync(astronautDetail);
                }
                else
                {
                    if (EnumHelper.IsRetired(dutyTitle.Title))
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                    }
                    _context.AstronautDetails.Update(astronautDetail);
                }

                var newAstronautDuty = new AstronautDuty
                {
                    PersonId = person.Id,
                    RankId = request.RankId,
                    DutyTitleId = request.DutyTitleId,
                    DutyStartDate = request.DutyStartDate.Date,
                    DutyEndDate = null
                };

                await _context.AstronautDuties.AddAsync(newAstronautDuty);
                await _context.SaveChangesAsync();

                await _loggingService.LogInfoAsync($"Successfully created astronaut duty with ID {newAstronautDuty.Id} for person: {request.Name}", "CreateAstronautDutyHandler", "Handle", requestData);

                return new CreateAstronautDutyResult()
                {
                    Id = newAstronautDuty.Id,
                    Success = true,
                    Message = "Astronaut duty created successfully"
                };
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 19) // UNIQUE constraint failed
            {
                await _loggingService.LogWarningAsync($"Unique constraint violation for person '{request.Name}' - person already has a current duty", "CreateAstronautDutyHandler", "Handle", requestData);
                return new CreateAstronautDutyResult()
                {
                    Id = null,
                    Success = false,
                    Message = "Person already has a current duty. Only one current duty is allowed per person.",
                    ResponseCode = 400
                };
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
            {
                await _loggingService.LogWarningAsync($"Database update exception - unique constraint violation for person '{request.Name}'", "CreateAstronautDutyHandler", "Handle", requestData);
                return new CreateAstronautDutyResult()
                {
                    Id = null,
                    Success = false,
                    Message = "Person already has a current duty. Only one current duty is allowed per person.",
                    ResponseCode = 400
                };
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync($"Unexpected error creating astronaut duty for person '{request.Name}'", ex, "CreateAstronautDutyHandler", "Handle", requestData);
                return new CreateAstronautDutyResult()
                {
                    Id = null,
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    ResponseCode = 500
                };
            }
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
