using System.Data.SqlClient;
using AZMapper;

namespace Example
{
    public class ConnectionConfiguration
    {
        public static ConnectionManager<SqlConnection> SqlConn;

        static ConnectionConfiguration()
        {
            SqlConn = new ConnectionManager<SqlConnection>(new DefaultConnectionFactory(), @"Server=TRUHLIK-NB\SQLEXPRESS;Database=Test;User Id=Tester;Password=heslo;MultipleActiveResultSets=True");
        }
    }
}
