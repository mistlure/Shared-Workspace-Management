using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateOccupancyDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Occupancy Id is required.")]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid UserId is required.")]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid WorkplaceId is required.")]
        public int WorkplaceId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Total price cannot be negative.")]
        public decimal? TotalPrice { get; set; }
    }
}
