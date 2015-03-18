using System.Data.SqlClient;
using AZMapper;

namespace Example
{
    #region MSSQL

    public class MssqlQuery : Query<SqlCommand, SqlParameter>
    {
        protected override System.Data.IDataReader GetParameterValueReader(object value)
        {
            throw new System.NotImplementedException();
        }
    }

    #endregion

    #region MSSQL Compact

    //public class SQLCompactQuery : Query<SqlCeCommand, SqlCeParameter>
    //{
    //    protected override System.Data.IDataReader GetParameterValueReader(object value)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}

    #endregion

    #region MySql

    //public class MysqlQuery : Query<MySqlCommand, MySqlParameter>
    //{
    //    protected override System.Data.IDataReader GetParameterValueReader(object value)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}

    #endregion

    #region Oracle

    //public class OracleQuery : Query<OracleCommand, OracleParameter>
    //{
    //    public OracleQuery()
    //    {
    //        _command = new OracleCommand();
    //        ((OracleCommand)_command).BindByName = true;
    //    }

    //    protected override IDataReader GetParameterValueReader(object value)
    //    {
    //        return ((OracleRefCursor)value).GetDataReader();
    //    }

    //    /// <summary>
    //    /// Only for stored procedures with output parameters
    //    /// </summary>
    //    /// <param name="parameterName">Name of the output parameter</param>
    //    /// <returns></returns>
    //    public decimal? ResultAsDecimal(string parameterName)
    //    {
    //        if (string.IsNullOrEmpty(parameterName))
    //            throw new ArgumentNullException("parameterName");

    //        var param = _command.Parameters[parameterName];
    //        var dec = ((Oracle.DataAccess.Types.OracleDecimal)param.Value);

    //        if (dec.IsNull)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return dec.Value;
    //        }
    //    }

    //    ///<summary>
    //    ///Only for stored procedures with output parameters
    //    ///</summary>
    //    ///<param name="parameterName">Name of the output parameter</param>
    //    ///<returns></returns>
    //    public DateTime? ResultAsDateTime(string parameterName)
    //    {
    //        if (string.IsNullOrEmpty(parameterName))
    //            throw new ArgumentNullException("parameterName");

    //        var param = _command.Parameters[parameterName];
    //        var date = ((Oracle.DataAccess.Types.OracleDate)param.Value);

    //        if (date.IsNull)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return date.Value;
    //        }
    //    }

    //    protected override void AddParameter(string name, int? type, int? size, object value, ParameterDirection direction)
    //    {
    //        if (_isCommandSet)
    //        {
    //            _command.Parameters[name].Value = value;
    //        }
    //        else
    //        {
    //            OracleParameter param = (OracleParameter)GetEmptyParameter();
    //            param.ParameterName = name;

    //            if (type.HasValue)
    //            {
    //                param.OracleDbType = (Oracle.DataAccess.Client.OracleDbType)type.Value;
    //            }

    //            param.Value = value;
    //            param.Direction = direction;

    //            if (size.HasValue)
    //            {
    //                param.Size = size.Value;
    //            }

    //            _command.Parameters.Add(param);
    //        }
    //    }
    //}

    #endregion

    #region SQLite

    //public class ConcreteQuery : Query<SqlCommand, SqlParameter>
    //{
    //    protected override System.Data.IDataReader GetParameterValueReader(object value)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public DataTable ResultAsDataTableDS()
    //    {
    //        var table = new DataTable();
    //        var da = new SQLiteDataAdapter((SQLiteCommand)_command);
    //        da.Fill(table);

    //        return table;
    //    }
    //}

    #endregion
}
