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
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        // Must be injected with a connection factory to create database connections
        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }



        public async Task<int> AddAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                INSERT INTO Users (FirstName, LastName, Email, PasswordHash, CreatedAt) 
                VALUES (@FirstName, @LastName, @Email, @PasswordHash, @CreatedAt);
                
                SELECT last_insert_rowid();";

            return await connection.QuerySingleAsync<int>(sql, user);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Users";

            return await connection.QueryAsync<User>(sql);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Users WHERE Email = @Email";

            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT * FROM Users WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
        }
    }
}
