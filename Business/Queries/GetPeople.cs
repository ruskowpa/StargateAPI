using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPeople : IRequest<GetPeopleResult>
    {

    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
    {
        public readonly StargateContext _context;
        public GetPeopleHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new GetPeopleResult();

                // Get all people and their astronaut detail info
                var query = $"SELECT a.Id as PersonId, a.Name, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id";

                var people = await _context.Connection.QueryAsync<PersonAstronaut>(query);

                // For each person, get their current rank and duty title from AstronautDuty table with JOINs
                foreach (var person in people)
                {
                    if (person != null)
                    {
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
                }

                result.People = people.ToList();
                result.Success = true;
                result.Message = "People retrieved successfully";

                return result;
            }
            catch (Exception ex)
            {
                return new GetPeopleResult()
                {
                    People = new List<PersonAstronaut>(),
                    Success = false,
                    Message = $"An error occurred while retrieving people: {ex.Message}",
                    ResponseCode = 500
                };
            }
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };

    }
}
