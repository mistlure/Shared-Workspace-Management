using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IStatusHistoryRepository
    {
        Task AddAsync(StatusHistory history);
        Task<IEnumerable<StatusHistory>> GetByWorkplaceIdAsync(int workplaceId);
        Task<IEnumerable<StatusHistory>> GetAllAsync();
    }
}