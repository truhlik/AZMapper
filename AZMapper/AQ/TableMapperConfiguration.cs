using System;
using System.Collections.Generic;

namespace AZMapper.AQ
{
    public class TableMapperConfiguration
    {
        private static Dictionary<Type, Table> _tables;
        public static string BindingValueMark { get; set; }

        static TableMapperConfiguration()
        {
            _tables = new Dictionary<Type, Table>();
            BindingValueMark = ":";
        }

        public void AddMapping(Type type, Table table)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if(table == null)
                throw new ArgumentNullException("table");

            if (_tables.ContainsKey(type))
                throw new MapperException(string.Format("Configuration for '{0}' already exists.", type.FullName));

            _tables.Add(type, table);
        }

        public Table Get(Type obj)
        {
            if (!_tables.ContainsKey(obj))
                throw new MapperException(string.Format("Configuration for '{0}' not found.", obj.FullName));

            return _tables[obj];
        }
    }
}
