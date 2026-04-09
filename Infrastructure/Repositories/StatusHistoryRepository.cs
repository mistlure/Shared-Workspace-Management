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
    public class StatusHistoryRepository : IStatusHistoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StatusHistoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }



        public async Task AddAsync(StatusHistory history)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                INSERT INTO StatusHistory (WorkplaceId, Status, ChangedAt)
                VALUES (@WorkplaceId, @Status, @ChangedAt);";

            await connection.ExecuteAsync(sql, history);
        }

        public async Task<IEnumerable<StatusHistory>> GetByWorkplaceIdAsync(int workplaceId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "SELECT * FROM StatusHistory WHERE WorkplaceId = @WorkplaceId ORDER BY ChangedAt DESC";
            return await connection.QueryAsync<StatusHistory>(sql, new { WorkplaceId = workplaceId });
        }
    }
}
