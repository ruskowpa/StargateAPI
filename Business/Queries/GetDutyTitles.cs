using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetDutyTitles : IRequest<GetDutyTitlesResult>
    {
    }

    public class GetDutyTitlesHandler : IRequestHandler<GetDutyTitles, GetDutyTitlesResult>
    {
        private readonly StargateContext _context;

        public GetDutyTitlesHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetDutyTitlesResult> Handle(GetDutyTitles request, CancellationToken cancellationToken)
        {
            try
            {
                var dutyTitles = await _context.DutyTitles
                    .Where(d => d.IsActive)
                    .OrderBy(d => d.Title)
                    .Select(d => new DutyTitleDto
                    {
                        Id = d.Id,
                        Title = d.Title,
                        Description = d.Description
                    })
                    .ToListAsync(cancellationToken);

                return new GetDutyTitlesResult
                {
                    DutyTitles = dutyTitles,
                    Success = true,
                    Message = "Duty titles retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new GetDutyTitlesResult
                {
                    DutyTitles = new List<DutyTitleDto>(),
                    Success = false,
                    Message = $"An error occurred while retrieving duty titles: {ex.Message}",
                    ResponseCode = 500
                };
            }
        }
    }

    public class GetDutyTitlesResult : BaseResponse
    {
        public List<DutyTitleDto> DutyTitles { get; set; } = new List<DutyTitleDto>();
    }

    public class DutyTitleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

