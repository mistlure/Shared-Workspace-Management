using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Occupancy
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WorkplaceId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
