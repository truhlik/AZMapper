using AZMapper;

namespace Example
{
    public class DataContextBase : DataContext<MssqlQuery>
    {
        public DataContextBase()
            : base(ConnectionConfiguration.SqlConn, true)
        {
        }
    }
}
