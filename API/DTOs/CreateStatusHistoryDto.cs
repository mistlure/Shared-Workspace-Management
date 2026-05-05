using Domain.Enums;

namespace API.DTOs
{
    public class CreateStatusHistoryDto
    {
        public int WorkplaceId { get; set; }
        public WorkplaceStatus Status { get; set; }
    }
}
