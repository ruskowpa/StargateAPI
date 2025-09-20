using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetRanks : IRequest<GetRanksResult>
    {
    }

    public class GetRanksHandler : IRequestHandler<GetRanks, GetRanksResult>
    {
        private readonly StargateContext _context;

        public GetRanksHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetRanksResult> Handle(GetRanks request, CancellationToken cancellationToken)
        {
            try
            {
                var ranks = await _context.Ranks
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Level)
                    .Select(r => new RankDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Abbreviation = r.Abbreviation,
                        Level = r.Level
                    })
                    .ToListAsync(cancellationToken);

                return new GetRanksResult
                {
                    Ranks = ranks,
                    Success = true,
                    Message = "Ranks retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new GetRanksResult
                {
                    Ranks = new List<RankDto>(),
                    Success = false,
                    Message = $"An error occurred while retrieving ranks: {ex.Message}",
                    ResponseCode = 500
                };
            }
        }
    }

    public class GetRanksResult : BaseResponse
    {
        public List<RankDto> Ranks { get; set; } = new List<RankDto>();
    }

    public class RankDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public int Level { get; set; }
    }
}

