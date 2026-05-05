namespace API.DTOs
{
    public class WorkspaceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxOccupationHours { get; set; }
    }
}
