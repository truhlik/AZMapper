using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AZMapper.Extensions;

namespace AZMapper.AQ
{
    public sealed class QueryGenerator
    {
        private static ConcurrentDictionary<string, string> _cache;

        static QueryGenerator()
        {
            _cache = new ConcurrentDictionary<string, string>();
        }

        public string Insert(Table tableMapping, Type objType, bool useCache = true)
        {
            return Generator("insert", objType, useCache, (type) =>
            {
                var part1 = new List<string>();
                var part2 = new List<string>();

                for (int i = 0; i < tableMapping.Fields.Count; i++)
                {
                    if (!tableMapping.Fields[i].AutoIncrement)
                    {
                        part1.Add(tableMapping.Fields[i].Name);
                        part2.Add(TableMapperConfiguration.BindingValueMark + tableMapping.Fields[i].Name);
                    }
                }

                return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableMapping.Name, string.Join(",", part1), string.Join(",", part2));
            }, null);
        }

        public string Update(Table tableMapping, Type objType, string[] fields, bool useCache)
        {
            return Generator("update", objType, useCache, (type) =>
            {
                var q = new List<string>();
                var primaryKey = new List<string>();

                for (int i = 0; i < tableMapping.Fields.Count; i++)
                {
                    string part = string.Format("{0}={1}{2}", tableMapping.Fields[i].Name, TableMapperConfiguration.BindingValueMark, tableMapping.Fields[i].Name);

                    if (tableMapping.Fields[i].IsPrimaryKey)
                    {
                        primaryKey.Add(part);
                    }
                    else
                    {
                        if (fields != null && fields.Length > 0)
                        {
                            if (fields.IsInArray(tableMapping.Fields[i].Name, false))
                            {
                                q.Add(part);
                            }
                        }
                        else
                        {
                            q.Add(part);
                        }
                    }
                }

                if (primaryKey.Count < 1)
                    throw new MapperException(string.Format("Table '{0}' does not have a primary key"));

                return string.Format("UPDATE {0} SET {1} WHERE {2}", tableMapping.Name, string.Join(",", q), string.Join(",", primaryKey));
            }, fields);
        }

        private string Generator(string queryName, Type objType, bool useCache, Func<Type, string> funcBuildQuery, string[] param)
        {
            string key = GetKey(objType, queryName, param);
            string query = string.Empty;

            return useCache ? _cache.GetOrAdd(key, funcBuildQuery(objType))
                : funcBuildQuery(objType);
        }

        private string GetKey(Type obj, string queryName, string[] param)
        {
            return queryName + obj.FullName + ((param != null && param.Length > 0) ? string.Join("_", param) : string.Empty);
        }
    }
}
