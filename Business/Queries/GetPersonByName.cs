using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;
using StargateAPI.Business.Services;
using System.Text.Json;

namespace StargateAPI.Business.Queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        private readonly StargateContext _context;
        private readonly ILoggingService _loggingService;
        
        public GetPersonByNameHandler(StargateContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            var requestData = JsonSerializer.Serialize(new { request.Name });
            await _loggingService.LogInfoAsync($"Retrieving person by name: {request.Name}", "GetPersonByNameHandler", "Handle", requestData);

            try
            {
                var result = new GetPersonByNameResult();

                // Get person and astronaut detail info
                var query = $"SELECT a.Id as PersonId, a.Name, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE '{request.Name}' = a.Name";

                var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(query);

                if (person != null)
                {
                    // Get current rank and duty title from AstronautDuty table with JOINs to reference tables
                    var currentDutyQuery = $@"
                        SELECT r.Abbreviation as CurrentRank, dt.Title as CurrentDutyTitle 
                        FROM [AstronautDuty] ad 
                        INNER JOIN [Rank] r ON ad.RankId = r.Id 
                        INNER JOIN [DutyTitle] dt ON ad.DutyTitleId = dt.Id 
                        WHERE ad.PersonId = {person.PersonId} AND ad.DutyEndDate IS NULL 
                        ORDER BY ad.DutyStartDate DESC LIMIT 1";
                    
                    var currentDuty = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(currentDutyQuery);
                    
                    if (currentDuty != null)
                    {
                        person.CurrentRank = currentDuty.CurrentRank;
                        person.CurrentDutyTitle = currentDuty.CurrentDutyTitle;
                    }
                }

                result.Person = person;
                result.Success = true;
                result.Message = person != null ? "Person retrieved successfully" : $"Person '{request.Name}' not found";

                if (person != null)
                {
                    await _loggingService.LogInfoAsync($"Successfully retrieved person: {request.Name}", "GetPersonByNameHandler", "Handle", requestData);
                }
                else
                {
                    await _loggingService.LogWarningAsync($"Person not found: {request.Name}", "GetPersonByNameHandler", "Handle", requestData);
                }

                return result;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync($"Error retrieving person: {request.Name}", ex, "GetPersonByNameHandler", "Handle", requestData);
                return new GetPersonByNameResult()
                {
                    Person = null,
                    Success = false,
                    Message = $"An error occurred while retrieving person '{request.Name}': {ex.Message}",
                    ResponseCode = 500
                };
            }
        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}
