using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Infrastructure.Data
{
    /// <summary>
    /// Zero Configuration Database Initializer
    /// (IF NOT EXISTS is used in the SQL statements to ensure that tables
    /// are only created if they do not already exist,
    /// preventing errors on subsequent runs)
    /// </summary>
    public static class DbInitializer
    {
        public static void Initialize(IDbConnectionFactory connectionFactory)
        {
            using var connection = connectionFactory.CreateConnection();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Workspaces (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Location TEXT NOT NULL,
                    MaxOccupationHours INTEGER NOT NULL,
                    PricePerHour REAL NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Workplaces (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    WorkspaceId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    CurrentStatus INTEGER NOT NULL,
                    FOREIGN KEY (WorkspaceId) REFERENCES Workspaces(Id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS Occupancy (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    WorkplaceId INTEGER NOT NULL,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT,
                    TotalPrice REAL NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (WorkplaceId) REFERENCES Workplaces(Id)
                );

                CREATE TABLE IF NOT EXISTS StatusHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    WorkplaceId INTEGER NOT NULL,
                    Status INTEGER NOT NULL,
                    ChangedAt TEXT NOT NULL,
                    FOREIGN KEY (WorkplaceId) REFERENCES Workplaces(Id) ON DELETE CASCADE
                );
            ";

            connection.Execute(sql);
        }
    }
}
