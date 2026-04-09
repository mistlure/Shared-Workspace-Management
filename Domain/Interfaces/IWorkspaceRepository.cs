using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IWorkspaceRepository
    {
        Task<Workspace?> GetByIdAsync(int id);
        Task<IEnumerable<Workspace>> GetAllAsync();
        Task<int> AddAsync(Workspace workspace);
        Task UpdateAsync(Workspace workspace);
        Task DeleteAsync(int id);
    }
}
