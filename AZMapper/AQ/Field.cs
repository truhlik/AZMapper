using System;
using System.Data;

namespace AZMapper.AQ
{
    public sealed class Field
    {
        public string Name { get; private set; }
        public bool IsPrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        public DbType DbType { get; set; }

        public Field(string name, bool isPrimaryKey, bool autoincrement, DbType type)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            AutoIncrement = autoincrement;
            this.DbType = type;
        }
    }
}
