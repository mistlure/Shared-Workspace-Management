namespace API.DTOs
{
    public class OccupancyResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WorkplaceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
