using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace AZMapper
{
    public abstract class DataContext<Q> : IDataContext, IDisposable
        where Q : Query, new()
    {
        private EventHandler _transactionCommitedEventHandler;
        private EventHandler _transactionRollbackedEventHandler;
        private EventHandler _transactionStartedEventHandler;
        private IDataContext _parent;

        private DbTransaction _transaction;
        private IConnectionManager _connManager;
        private bool _autonomous;
        private bool _isDisposed;

        #region Properties

        public event EventHandler TransactionStarted
        {
            add { _transactionStartedEventHandler += value; }
            remove { _transactionStartedEventHandler -= value; }
        }

        public event EventHandler TransactionRollbacked
        {
            add { _transactionRollbackedEventHandler += value; }
            remove { _transactionRollbackedEventHandler -= value; }
        }

        public event EventHandler TransactionCommited
        {
            add { _transactionCommitedEventHandler += value; }
            remove { _transactionCommitedEventHandler -= value; }
        }

        public DbTransaction Transaction
        {
            get { return _transaction; }
        }

        public DbConnection Connection
        {
            get { return _connManager.GetConnection(); }
        }

        #endregion

        #region Constructors

        protected DataContext(IConnectionManager connManager, bool autonomous = true)
        {
            if (connManager == null)
                throw new ArgumentNullException("connManager");

            _connManager = autonomous ? connManager.CreateInstance() : connManager;
            _isDisposed = false;
            _parent = null;
            _autonomous = autonomous;
        }

        protected DataContext(IConnectionManager connManager, IDataContext parent, bool autonomous = true)
            : this(connManager, autonomous)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            _parent = parent;

            _transactionCommitedEventHandler = (object sender, EventArgs args) => { CommitTransaction(); };
            _transactionRollbackedEventHandler = (object sender, EventArgs args) => { Rollback(); };
            _transactionStartedEventHandler = (object sender, EventArgs args) => { BeginTransaction(); };

            parent.TransactionCommited += _transactionCommitedEventHandler;
            parent.TransactionRollbacked += _transactionRollbackedEventHandler;
            parent.TransactionStarted += _transactionStartedEventHandler;
        }

        #endregion

        #region Transaction

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
            }

            if (_transactionRollbackedEventHandler != null)
                _transactionRollbackedEventHandler(this, EventArgs.Empty);
        }

        public void BeginTransaction()
        {
            var connection = _connManager.GetConnection();
            if (connection.State == ConnectionState.Closed)
                OpenConnection();

            if (_transaction != null)
            {
                _transaction.Dispose();
            }

            _transaction = connection.BeginTransaction();

            if (_transactionStartedEventHandler != null)
                _transactionStartedEventHandler(this, EventArgs.Empty);
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
            }

            if (_transactionCommitedEventHandler != null)
                _transactionCommitedEventHandler(this, EventArgs.Empty);
        }

        #endregion

        #region Connection

        public void CloseConnection()
        {
            _connManager.CloseConnection();
        }

        public void OpenConnection()
        {
            _connManager.OpenConnection();
        }

        #endregion

        #region Common

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_autonomous)
                    {
                        if (_transaction != null)
                        {
                            _transaction.Dispose();
                        }

                        if (_connManager != null)
                        {
                            _connManager.Dispose();
                        }
                    }

                    if (_parent != null)
                    {
                        //unregister all events if needed
                        if (_transactionCommitedEventHandler != null)
                            _parent.TransactionCommited -= _transactionCommitedEventHandler;
                        if (_transactionRollbackedEventHandler != null)
                            _parent.TransactionRollbacked -= _transactionRollbackedEventHandler;
                        if (_transactionStartedEventHandler != null)
                            _parent.TransactionStarted -= _transactionStartedEventHandler;
                    }
                }

                _transaction = null;
                _connManager = null;
                _isDisposed = true;
                _parent = null;
                //
                _transactionCommitedEventHandler = null;
                _transactionRollbackedEventHandler = null;
                _transactionStartedEventHandler = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        #endregion

        #region Query

        /// <summary>
        /// Insert/Update
        /// </summary>
        /// <param name="allowTransaction"></param>
        /// <returns></returns>
        public Query CreateQuery(bool allowTransaction = true)
        {
            Query q = new Q();
            q.Connection = this.Connection;
            q.Transaction = allowTransaction ? this._transaction : null;
            return q;
        }

        public Query CreateQuery(string package, string proc, bool allowTransaction = true)
        {
            var q = CreateQuery(allowTransaction);
            q.PrepareCommand(package, proc);
            return q;
        }

        public Query CreateQuery(string query, bool allowTransaction = true)
        {
            var q = CreateQuery(allowTransaction);
            q.PrepareCommand(query);
            return q;
        }

        #endregion

        #region Generic methods

        public virtual int Update<Tin>(Tin obj, string[] fields = null) where Tin : class, new()
        {
            using (var q = this.CreateQuery())
            {
                return q.Update<Tin>(obj, fields);
            }
        }

        public virtual int WithMonitor<Tin>(Tin obj) where Tin : class, new()
        {
            using (var q = this.CreateQuery())
            {
                return q.UpdateWithMonitor(obj);
            }
        }

        public virtual int BulkInsert<Tin>(IEnumerable<Tin> source) where Tin : class
        {
            using (var q = this.CreateQuery())
            {
                return q.BulkInsert<Tin>(source);
            }
        }

        public virtual int Insert<Tin>(Tin obj) where Tin : class
        {
            using (var q = this.CreateQuery())
            {
                return q.Insert<Tin>(obj);
            }
        }

        #endregion
    }
}
