using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateOccupancyDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid UserId is required.")]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid WorkplaceId is required.")]
        public int WorkplaceId { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool NeedsMonitor { get; set; }

        [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? SpecialRequests { get; set; }
    }
}
