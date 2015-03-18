using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using AZMapper.AQ;
using AZMapper.Extensions;

namespace AZMapper
{
    public abstract class Query : IDisposable
    {
        protected DbCommand _command;
        protected IDataReader _reader;
        protected bool _isCommandSet;
        public bool UseAutoMapper { get; set; }
        private bool _isDisposed;

        #region Properties

        public int RowsAffected { get; private set; }

        public DbConnection Connection
        {
            get { return _command.Connection; }
            set { _command.Connection = value; }
        }

        public DbTransaction Transaction
        {
            get { return _command.Transaction; }
            set { _command.Transaction = value; }
        }

        #endregion

        #region Constructors

        protected Query()
        {
            UseAutoMapper = true;
            _isCommandSet = false;
            _command = null;
            _reader = null;
            _isDisposed = false;
            RowsAffected = -1;
        }

        public Query(string query)
            : this()
        {
            _command.CommandType = CommandType.Text;
            _command.CommandText = query;
        }

        protected Query(string package, string storedProcedure)
            : this()
        {
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = string.Format("{0}.{1}", package, storedProcedure);
        }

        #endregion

        #region Command

        public void PrepareCommand(string query)
        {
            _command.CommandType = CommandType.Text;
            _command.CommandText = query;
        }

        public void PrepareCommand(string package, string storedProcedure)
        {
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = string.Format("{0}.{1}", package, storedProcedure);
        }

        #endregion

        #region Select methods

        /// <summary>
        /// returns a list of a single field in output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field within a resultset</param>
        /// <param name="conversionMethod"></param>
        /// <param name="outputParameter">Name of the output command parameter used to retrieve related datareader</param>
        /// <returns> </returns>
        public List<T> ResultAsList<T>(string fieldName, Func<IDataReader, int, T> conversionMethod, string outputParameter = null)
        {
            var list = new List<T>();
            foreach (var item in ResultAsEnumerable<T>(fieldName, conversionMethod, outputParameter))
            {
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// returns an enumaration of a single field in output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field within a resultset</param>
        /// <param name="conversionMethod">A function used to manipulate with the data</param>
        /// <param name="outputParameter">Name of the output command parameter used to retrieve related datareader</param>
        /// <returns> </returns>
        public IEnumerable<T> ResultAsEnumerable<T>(string fieldName, Func<IDataReader, int, T> conversionMethod, string outputParameter = null)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("fieldName");
            if (conversionMethod == null)
                throw new ArgumentNullException("conversionMethod");
            if (_reader == null && string.IsNullOrEmpty(outputParameter))
                throw new ArgumentNullException("outputParameter", "In case of 'ExecuteNonQuery' parameter name cannot be null ");

            IDataReader r = _reader ?? GetParameterValueReader(_command.Parameters[outputParameter].Value);
            try
            {
                if (r != null && !r.IsClosed)
                {
                    int idx = r.GetOrdinal(fieldName);
                    while (r.Read())
                    {
                        yield return conversionMethod(r, idx);
                    }
                }
            }
            finally
            {
                if (_reader == null && r != null)
                    r.Dispose();
            }
        }

        /// <summary>
        /// returns a list of a single field in output.
        /// </summary>
        /// <param name="fieldName">Name of the field within a resultset</param>
        /// <param name="outputParameter">Name of the output command parameter used to retrieve related datareader</param>
        /// <returns>An object list of the database field type</returns>
        public List<object> ResultAsList(string fieldName, string outputParameter = null)
        {
            var list = new List<object>();
            foreach (var item in ResultAsEnumerable(fieldName, outputParameter))
            {
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// returns an enumaration of a single field in output.
        /// </summary>
        /// <param name="fieldName">Name of the field within output resultset</param>
        /// <param name="outputParameter">Name of the output command parameter used to retrieve related datareader</param>
        /// <returns> An numeration of the database field type</returns>
        public IEnumerable<object> ResultAsEnumerable(string fieldName, string outputParameter = null)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("fieldName");
            if (_reader == null && string.IsNullOrEmpty(outputParameter))
                throw new ArgumentNullException("outputParameter", "In case of 'ExecuteNonQuery' parameter cannot be null ");

            IDataReader r = _reader ?? GetParameterValueReader(_command.Parameters[outputParameter].Value);
            try
            {
                if (r != null && !r.IsClosed)
                {
                    while (r.Read())
                    {
                        yield return r[fieldName];
                    }
                }
            }
            finally
            {
                if (_reader == null && r != null)
                    r.Dispose();
            }
        }

        /// <summary>
        /// Function to obtain IDataReader from the command (output) parameter
        /// </summary>
        /// <param name="value">Value of the output parameter</param>
        /// <returns>IDataReader</returns>
        protected abstract IDataReader GetParameterValueReader(object value);

        /// <summary>
        /// Fill the table with the data
        /// </summary>
        /// <param name="outputParameter">Name of the output command parameter used to retrieve related datareader</param>
        /// <returns> </returns>
        public virtual DataTable ResultAsDataTable(string outputParameter = null)
        {
            if (_reader == null && string.IsNullOrEmpty(outputParameter))
                throw new ArgumentNullException("outputParameter", "In case of 'ExecuteNonQuery' parameter cannot be null");

            var table = new DataTable();
            IDataReader r = _reader ?? GetParameterValueReader(_command.Parameters[outputParameter].Value);

            try
            {
                if (r != null && !r.IsClosed)
                {
                    table.Load(r, LoadOption.OverwriteChanges);
                }
            }
            finally
            {
                if (_reader == null && r != null)
                    r.Dispose();
            }

            return table;
        }

        /// <summary>
        /// returns an enumeration of the custom entity using the automapper or the provided entity mapping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="outputParameter">Name of the output command parameter used to retrieve related datareader</param>
        /// <param name="action">An action executed after mapping used to extend its mapping behaviour</param>
        /// <returns></returns>
        public IEnumerable<T> ResultAsEnumerable<T>(string outputParameter = null, Action<T> action = null) where T : class, new()
        {
            if (_reader == null && string.IsNullOrEmpty(outputParameter))
                throw new ArgumentNullException("outputParameter", "In case of 'ExecuteNonQuery' parameter cannot be null");

            IDataReader r = _reader ?? GetParameterValueReader(_command.Parameters[outputParameter].Value);
            try
            {
                if (r != null && !r.IsClosed)
                {
                    ResultEntityMapper<T> entity = UseAutoMapper ? new ResultEntityMapper<T>() : null;
                    int hashCode = this.GetHashCode();

                    if (!UseAutoMapper && !typeof(IMapable).IsAssignableFrom(typeof(T)))
                    {
                        throw new MapperException("Object doesn't implement IMapable interface");
                    }

                    while (r.Read())
                    {
                        var instance = new T();

                        if (UseAutoMapper)
                        {
                            entity.Bind(r, hashCode, instance);
                        }
                        else
                        {
                            ((IMapable)instance).Map(r);
                        }

                        if (action != null)
                            action(instance);

                        yield return instance;
                    }
                }
            }
            finally
            {
                if (_reader == null && r != null)
                    r.Dispose();
            }
        }


        /// <summary>
        /// returns a list of the custom entity using the automapper or the provided entity mapping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="outputParameter">Name of the output command parameter used to retrieve related datareader</param>
        /// <param name="action">An action executed after mapping used to extend mapping behaviour</param>
        /// <returns></returns>
        public List<T> ResultAsList<T>(string outputParameter = null, Action<T> action = null) where T : class, new()
        {
            var list = new List<T>();
            foreach (var item in ResultAsEnumerable<T>(outputParameter, action))
            {
                list.Add(item);
            }

            return list;
        }


        /// <summary>
        /// returns a value from the command parameter
        /// </summary>
        /// <param name="outputParameter">parameter name</param>
        /// <returns></returns>
        public object GetOutputValue(string outputParameter)
        {
            return _command.Parameters[outputParameter].Value;
        }

        /// <summary>
        /// returns a value from the command parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="outputParameter">parameter name</param>
        /// <returns></returns>
        public T GetOutputValue<T>(string outputParameter)
        {
            return (T)_command.Parameters[outputParameter].Value;
        }

        public void NextResult()
        {
            if (_reader != null && !_reader.IsClosed)
                _reader.NextResult();
        }

        #endregion

        #region Common

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_reader != null)
                    {
                        _reader.Dispose();
                    }
                    if (_command != null)
                    {
                        _command.Dispose();
                    }
                }

                _reader = null;
                _command = null;
                _isDisposed = true;
            }
        }

        public void ClearCommandParameters()
        {
            if (_command != null)
                _command.Parameters.Clear();
        }

        /// <summary>
        /// Execute command/query
        /// </summary>
        public void ExecuteNonQuery()
        {
            _isCommandSet = true;
            RowsAffected = _command.ExecuteNonQuery();
        }

        public void ExecuteReader()
        {
            _reader = _command.ExecuteReader();
            RowsAffected = _reader.RecordsAffected;
        }

        public override int GetHashCode()
        {
            return _command.CommandText.GetHashCode();
        }

        #endregion

        #region Parameters

        public Query AddOutputParameter(string name, int? type = null, int? size = null)
        {
            AddParameter(name, type, size, DBNull.Value, ParameterDirection.Output);
            return this;
        }

        public Query AddInputParameter(string name, object value, int? type = null, int? size = null)
        {
            AddParameter(name, type, size, value, ParameterDirection.Input);
            return this;
        }

        protected abstract DbParameter GetEmptyParameter();

        protected virtual void AddParameter(string name, int? type, int? size, object value, ParameterDirection direction)
        {
            if (_isCommandSet)
            {
                _command.Parameters[name].Value = (value == null) ? DBNull.Value : value;
            }
            else
            {
                var param = GetEmptyParameter();
                param.ParameterName = name;

                if (type.HasValue)
                {
                    param.DbType = (DbType)type.Value;
                }

                param.Value = (value == null) ? DBNull.Value : value;
                param.Direction = direction;

                if (size.HasValue)
                {
                    param.Size = size.Value;
                }

                _command.Parameters.Add(param);
            }
        }

        #endregion

        #region Insert/update

        public int Insert<T>(T obj) where T : class
        {
            var qg = new QueryGenerator();
            var getters = ObjectGetterStorage.Instance.GetGetters(typeof(T));
            var mc = new TableMapperConfiguration();
            var tableMap = mc.Get(typeof(T));

            string query = qg.Insert(tableMap, typeof(T), true);
            PrepareCommand(query);

            for (int i = 0; i < tableMap.Fields.Count; i++)
            {
                if (!tableMap.Fields[i].AutoIncrement)
                    AddInputParameter(tableMap.Fields[i].Name, getters[tableMap.Fields[i].Name].Getter(obj), (int)tableMap.Fields[i].DbType);
            }

            this.ExecuteNonQuery();

            return RowsAffected;
        }

        public int BulkInsert<T>(IEnumerable<T> source) where T : class
        {
            var qg = new QueryGenerator();
            var getters = ObjectGetterStorage.Instance.GetGetters(typeof(T));
            var mc = new TableMapperConfiguration();
            var tableMap = mc.Get(typeof(T));
            int insertedRows = 0;

            string query = qg.Insert(tableMap, typeof(T), true);
            PrepareCommand(query);

            foreach (var item in source)
            {
                for (int i = 0; i < tableMap.Fields.Count; i++)
                {
                    if (!tableMap.Fields[i].AutoIncrement)
                        AddInputParameter(tableMap.Fields[i].Name, getters[tableMap.Fields[i].Name].Getter(item), (int)tableMap.Fields[i].DbType);
                }

                this.ExecuteNonQuery();
                insertedRows += this.RowsAffected;
            }

            RowsAffected = insertedRows;
            return insertedRows;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="obj"></param>
        /// <param name="fields">Fields from the target table</param>
        /// <returns></returns>
        public int Update<T>(T obj, string[] fields = null) where T : class
        {
            if (fields != null && fields.Length == 0)
                fields = null;

            var qg = new QueryGenerator();
            var getters = ObjectGetterStorage.Instance.GetGetters(typeof(T));
            var mc = new TableMapperConfiguration();
            var tableMap = mc.Get(typeof(T));

            string query = qg.Update(tableMap, typeof(T), fields, true);
            PrepareCommand(query);

            for (int i = 0; i < tableMap.Fields.Count; i++)
            {
                if (fields == null || tableMap.Fields[i].IsPrimaryKey || (fields != null && fields.IsInArray(tableMap.Fields[i].Name, false)))
                {
                    AddInputParameter(tableMap.Fields[i].Name, getters[tableMap.Fields[i].Name].Getter(obj), (int)tableMap.Fields[i].DbType);
                }
            }

            this.ExecuteNonQuery();

            return RowsAffected;
        }

        public int UpdateWithMonitor<T>(T obj) where T : class
        {
            var diff = EntityMonitor.Instance.GetDiff(obj);

            if (diff != null && diff.Count > 0)
            {
                return Update<T>(obj, diff.ToArray());
            }

            return 0;
        }

        #endregion
    }
}
