namespace API.DTOs
{
    public class CreateOccupancyDto
    {
        public int WorkplaceId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
