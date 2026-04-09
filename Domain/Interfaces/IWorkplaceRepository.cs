using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IWorkplaceRepository
    {
        Task<Workplace?> GetByIdAsync(int id);
        Task<IEnumerable<Workplace>> GetByWorkspaceIdAsync(int workspaceId);
        Task<IEnumerable<Workplace>> GetAllAsync();
        Task<int> AddAsync(Workplace workplace);
        Task UpdateAsync(Workplace workplace);
        Task DeleteAsync(int id);
    }
}
