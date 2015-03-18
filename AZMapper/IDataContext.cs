using System;
using System.Data.Common;

namespace AZMapper
{
    public interface IDataContext
    {
        DbConnection Connection { get; }
        DbTransaction Transaction { get; }

        event EventHandler TransactionStarted;
        event EventHandler TransactionRollbacked;
        event EventHandler TransactionCommited;
    }
}
