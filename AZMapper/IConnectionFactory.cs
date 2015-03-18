using System.Data.Common;

namespace AZMapper
{
    public interface IConnectionFactory
    {
        T Create<T>(string connectionString) where T : DbConnection, new();
    }
}
