using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using StargateAPI.Business.Services;
using System.Text.Json;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly StargateContext _context;
        private readonly ILoggingService _loggingService;
        
        public CreatePersonPreProcessor(StargateContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }
        
        public async Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            var requestData = JsonSerializer.Serialize(new { request.Name });
            await _loggingService.LogInfoAsync($"Starting person creation validation for: {request.Name}", "CreatePersonPreProcessor", "Process", requestData);

            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person is not null) 
            {
                await _loggingService.LogWarningAsync($"Duplicate person creation attempt for name: {request.Name}", "CreatePersonPreProcessor", "Process", requestData);
                throw new BadHttpRequestException("A person with this name already exists. Person names must be unique.", 400);
            }

            await _loggingService.LogInfoAsync($"Validation successful for person creation: {request.Name}", "CreatePersonPreProcessor", "Process", requestData);
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        private readonly StargateContext _context;
        private readonly ILoggingService _loggingService;

        public CreatePersonHandler(StargateContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }
        
        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var requestData = JsonSerializer.Serialize(new { request.Name });
            await _loggingService.LogInfoAsync($"Starting person creation for: {request.Name}", "CreatePersonHandler", "Handle", requestData);

            try
            {
                var newPerson = new Person()
                {
                   Name = request.Name
                };

                await _context.People.AddAsync(newPerson);
                await _context.SaveChangesAsync();

                await _loggingService.LogInfoAsync($"Successfully created person with ID {newPerson.Id} for name: {request.Name}", "CreatePersonHandler", "Handle", requestData);

                return new CreatePersonResult()
                {
                    Id = newPerson.Id,
                    Success = true,
                    Message = "Person created successfully"
                };
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
            {
                await _loggingService.LogWarningAsync($"Database constraint violation - duplicate person name: {request.Name}", "CreatePersonHandler", "Handle", requestData);
                return new CreatePersonResult()
                {
                    Id = 0,
                    Success = false,
                    Message = "A person with this name already exists. Person names must be unique.",
                    ResponseCode = 400
                };
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync($"Unexpected error creating person: {request.Name}", ex, "CreatePersonHandler", "Handle", requestData);
                return new CreatePersonResult()
                {
                    Id = 0,
                    Success = false,
                    Message = $"An error occurred while creating the person: {ex.Message}",
                    ResponseCode = 500
                };
            }
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
