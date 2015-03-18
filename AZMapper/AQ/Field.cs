using System;
using System.Data;

namespace AZMapper.AQ
{
    public sealed class Field
    {
        public string FieldTable { get; private set; }
        public bool IsPrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        public DbType DbType { get; set; }

        public Field(string fieldTable, bool isPrimaryKey, bool autoincrement, DbType type)
        {
            FieldTable = fieldTable;
            IsPrimaryKey = isPrimaryKey;
            AutoIncrement = autoincrement;
            this.DbType = type;
        }
    }
}
