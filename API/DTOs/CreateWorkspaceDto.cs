using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateWorkspaceDto
    {
        [Required(ErrorMessage = "Workspace name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(1, 24, ErrorMessage = "Max occupation hours must be between 1 and 24.")]
        public int MaxOccupationHours { get; set; }

        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Price per hour must be strictly positive.")]
        public decimal PricePerHour { get; set; }
    }
}
