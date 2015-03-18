using System;
using System.Data.Common;

namespace AZMapper
{
    public interface IConnectionManager : IDisposable
    {
        void OpenConnection();
        void CloseConnection();
        string ConnectionString { get; set; }
        IConnectionManager CreateInstance();
        DbConnection GetConnection();
    }
}
