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
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public WorkspaceRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }



        public async Task<int> AddAsync(Workspace workspace)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                INSERT INTO Workspaces (Name, Location, MaxOccupationHours, PricePerHour)
                VALUES (@Name, @Location, @MaxOccupationHours, @PricePerHour);
                
                SELECT last_insert_rowid();";

            return await connection.QuerySingleAsync<int>(sql, workspace);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "DELETE FROM Workspaces WHERE Id = @Id";

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Workspace>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Workspaces";

            return await connection.QueryAsync<Workspace>(sql);
        }

        public async Task<Workspace?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Workspaces WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Workspace>(sql, new { Id = id });
        }

        public async Task UpdateAsync(Workspace workspace)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                UPDATE Workspaces 
                SET Name = @Name, 
                    Location = @Location, 
                    MaxOccupationHours = @MaxOccupationHours, 
                    PricePerHour = @PricePerHour 
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, workspace);
        }
    }
}
