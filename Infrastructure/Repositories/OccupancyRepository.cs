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
    public class OccupancyRepository : IOccupancyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OccupancyRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }



        public async Task<int> AddAsync(Occupancy occupancy)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                INSERT INTO Occupancy (UserId, WorkplaceId, StartTime, EndTime, TotalPrice)
                VALUES (@UserId, @WorkplaceId, @StartTime, @EndTime, @TotalPrice);
                
                SELECT last_insert_rowid();";

            return await connection.QuerySingleAsync<int>(sql, occupancy);
        }

        public async Task<Occupancy?> GetActiveByUserIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Occupancy WHERE UserId = @UserId AND EndTime IS NULL";

            return await connection.QueryFirstOrDefaultAsync<Occupancy>(sql, new { UserId = userId });
        }

        public async Task<Occupancy?> GetActiveByWorkplaceIdAsync(int workplaceId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Occupancy WHERE WorkplaceId = @WorkplaceId AND EndTime IS NULL";

            return await connection.QueryFirstOrDefaultAsync<Occupancy>(sql, new { WorkplaceId = workplaceId });
        }

        public async Task<IEnumerable<Occupancy>> GetAllActiveAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            string sql = "SELECT * FROM Occupancy WHERE EndTime IS NULL";

            return await connection.QueryAsync<Occupancy>(sql);
        }

        public async Task<Occupancy?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Occupancy WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Occupancy>(sql, new { Id = id });
        }

        public async Task UpdateAsync(Occupancy occupancy)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                UPDATE Occupancy 
                SET UserId = @UserId, 
                    WorkplaceId = @WorkplaceId, 
                    StartTime = @StartTime, 
                    EndTime = @EndTime, 
                    TotalPrice = @TotalPrice 
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, occupancy);
        }
    }
}
