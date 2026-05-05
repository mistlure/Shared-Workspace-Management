using Domain.Enums;

namespace API.DTOs
{
    public class WorkplaceResponseDto
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public WorkplaceStatus CurrentStatus { get; set; }
    }
}
