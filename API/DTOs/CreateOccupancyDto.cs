namespace API.DTOs
{
    public class CreateOccupancyDto
    {
        public int UserId { get; set; }
        public int WorkplaceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
