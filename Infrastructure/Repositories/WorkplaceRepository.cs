using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class WorkplaceRepository : IWorkplaceRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public WorkplaceRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }



        public async Task<int> AddAsync(Workplace workplace)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO Workplaces (WorkspaceId, Name, CurrentStatus)
                VALUES (@WorkspaceId, @Name, @CurrentStatus);
                
                SELECT last_insert_rowid();";

            return await connection.QuerySingleAsync<int>(sql, workplace);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "DELETE FROM Workplaces WHERE Id = @Id";

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Workplace>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Workplaces";

            return await connection.QueryAsync<Workplace>(sql);
        }

        public async Task<Workplace?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Workplaces WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Workplace>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Workplace>> GetByWorkspaceIdAsync(int workspaceId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Workplaces WHERE WorkspaceId = @WorkspaceId";

            return await connection.QueryAsync<Workplace>(sql, new { WorkspaceId = workspaceId });
        }

        public async Task UpdateAsync(Workplace workplace)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                UPDATE Workplaces 
                SET WorkspaceId = @WorkspaceId, 
                    Name = @Name, 
                    CurrentStatus = @CurrentStatus 
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, workplace);
        }
    }
}
