using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateWorkplaceDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid WorkspaceId is required.")]
        public int WorkspaceId { get; set; }

        [Required(ErrorMessage = "Workplace name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name is required and cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
