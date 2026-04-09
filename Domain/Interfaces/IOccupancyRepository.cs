using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IOccupancyRepository
    {
        Task<Occupancy?> GetByIdAsync(int id);
        Task<IEnumerable<Occupancy>> GetAllActiveAsync();
        Task<Occupancy?> GetActiveByUserIdAsync(int userId);
        Task<Occupancy?> GetActiveByWorkplaceIdAsync(int workplaceId);
        Task<int> AddAsync(Occupancy occupancy);
        Task UpdateAsync(Occupancy occupancy);
    }
}
