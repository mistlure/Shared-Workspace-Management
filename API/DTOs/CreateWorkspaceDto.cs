namespace API.DTOs
{
    public class CreateWorkspaceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int MaxOccupationHours { get; set; }
        public decimal PricePerHour { get; set; }
    }
}
