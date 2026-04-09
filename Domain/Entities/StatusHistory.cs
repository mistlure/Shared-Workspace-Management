using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class StatusHistory
    {
        public int Id { get; set; }
        public int WorkplaceId { get; set; }
        public WorkplaceStatus Status { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
