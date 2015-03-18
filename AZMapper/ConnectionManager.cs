using System;
using System.Data.Common;

namespace AZMapper
{
    public class ConnectionManager<T> : IConnectionManager where T : DbConnection, new()
    {
        private T _connection;
        private IConnectionFactory _connectionFactory;
        public string ConnectionString { get; set; }
        private bool _isDisposed;

        public ConnectionManager(IConnectionFactory factory, string connectionString)
        {
            _connectionFactory = factory;
            this.ConnectionString = connectionString;
            _isDisposed = false;
        }

        public DbConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = _connectionFactory.Create<T>(ConnectionString);
            }

            return _connection;
        }

        public void OpenConnection()
        {
            if (_connection == null)
                _connection = _connectionFactory.Create<T>(ConnectionString);

            _connection.Open();
        }

        public void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        public IConnectionManager CreateInstance()
        {
            return new ConnectionManager<T>(_connectionFactory, ConnectionString);
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                if (disposing)
                {
                    CloseConnection();
                }

                _connectionFactory = null;
            }
        }

        #endregion
    }
}
