using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace API.DTOs
{
    public class CreateStatusHistoryDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid WorkplaceId is required.")]
        public int WorkplaceId { get; set; }

        [Required]
        [EnumDataType(typeof(WorkplaceStatus), ErrorMessage = "Invalid status value.")]
        public WorkplaceStatus Status { get; set; }
    }
}
