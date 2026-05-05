namespace API.DTOs
{
    public class WorkspaceResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int MaxOccupationHours { get; set; }
        public decimal PricePerHour { get; set; }
    }
}
