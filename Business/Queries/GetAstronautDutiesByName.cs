using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDutiesByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new GetAstronautDutiesByNameResult();

                // Get person and astronaut detail info
                var query = $"SELECT a.Id as PersonId, a.Name, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE \'{request.Name}\' = a.Name";

                var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(query);

                if (person != null)
                {
                    // Get current rank and duty title from AstronautDuty table with JOINs
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

                    // Get all duties with JOINs to get rank and duty title names
                    query = $@"
                        SELECT ad.Id, ad.PersonId, ad.DutyStartDate, ad.DutyEndDate,
                               r.Abbreviation as Rank, dt.Title as DutyTitle
                        FROM [AstronautDuty] ad 
                        INNER JOIN [Rank] r ON ad.RankId = r.Id 
                        INNER JOIN [DutyTitle] dt ON ad.DutyTitleId = dt.Id 
                        WHERE ad.PersonId = {person.PersonId} 
                        ORDER BY ad.DutyStartDate DESC";

                    var duties = await _context.Connection.QueryAsync<AstronautDuty>(query);

                    result.AstronautDuties = duties.ToList();
                }
                else
                {
                    result.AstronautDuties = new List<AstronautDuty>();
                }

                result.Person = person;
                result.Success = true;
                result.Message = person != null ? $"Astronaut duties retrieved successfully for {request.Name}" : $"Person '{request.Name}' not found";

                return result;
            }
            catch (Exception ex)
            {
                return new GetAstronautDutiesByNameResult()
                {
                    Person = null,
                    AstronautDuties = new List<AstronautDuty>(),
                    Success = false,
                    Message = $"An error occurred while retrieving astronaut duties for '{request.Name}': {ex.Message}",
                    ResponseCode = 500
                };
            }
        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public PersonAstronaut Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}
