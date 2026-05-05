using Domain.Enums;

namespace API.DTOs
{
    public class StatusHistoryResponseDto
    {
        public int Id { get; set; }
        public int WorkplaceId { get; set; }
        public WorkplaceStatus Status { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
