using System.Data;
using System.Data.SqlClient;

namespace refactor_me.Database
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}