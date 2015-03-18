using System.Data.Common;

namespace AZMapper
{
    public class DefaultConnectionFactory : IConnectionFactory
    {
        public T Create<T>(string connectionString) where T : DbConnection, new()
        {
            var connection = new T();
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
