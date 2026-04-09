using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    /// <summary>
    /// Contract for a factory that creates database connections.
    /// This interface abstracts the creation of database connections,
    /// allowing for flexibility in how connections are established
    /// and managed within the application.
    /// </summary>
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
