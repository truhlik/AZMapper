using System;
using System.Collections.Generic;
using System.Data;

namespace AZMapper.AQ
{
    public sealed class Table
    {
        public string Name { get; private set; }
        public List<Field> Fields { get; private set; }
        public IDbTypeResolver _dbTypeResolver;

        public Table(string name, IDbTypeResolver resolver = null)
        {
            Fields = new List<Field>();
            Name = name;
            _dbTypeResolver = resolver;
        }

        public Table AddField(string tableFieldName, DbType type = DbType.AnsiString, bool isPrimaryKey = false, bool autoincrement = false)
        {
            Fields.Add(new Field(tableFieldName, isPrimaryKey, autoincrement, type));
            return this;
        }

        public Table AddFieldsAuto(Type type)
        {
            var getters = ObjectGetterStorage.Instance.GetGetters(type);
            foreach (var g in getters)
            {
                Fields.Add(new Field(g.Key, false, false, _dbTypeResolver.FromType(g.Value.ReturnType)));
            }

            return this;
        }

        public Table SetPrimaryKey(string tableFieldName, bool autoincrement = false)
        {
            tableFieldName = tableFieldName.Trim().ToLower();
            for (int i = 0; i < Fields.Count; i++)
            {
                if (Fields[i].Name.ToLower().Equals(tableFieldName))
                {
                    var item = Fields[i];
                    item.IsPrimaryKey = true;
                    item.AutoIncrement = autoincrement;
                    break;
                }
            }

            return this;
        }
    }
}
