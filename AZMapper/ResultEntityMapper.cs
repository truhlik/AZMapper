using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using AZMapper.Extensions;

namespace AZMapper
{
    public sealed partial class ResultEntityMapper<T> where T : class, new()
    {
        private Dictionary<int, List<FieldInfoMapper>> _mappers;
        private static Dictionary<string, Action<T, IDataReader, int>> _properties;

        public ResultEntityMapper()
        {
            _mappers = new Dictionary<int, List<FieldInfoMapper>>();
        }

        static ResultEntityMapper()
        {
            CreateProperties();
        }

        private static void CreateProperties()
        {
            Type t = typeof(T);
            var properties = t.GetProperties();
            _properties = new Dictionary<string, Action<T, IDataReader, int>>(properties.Length);

            for (int i = 0; i < properties.Length; i++)
            {
                AddPropertySetter(properties[i]);
            }
        }

        public void Bind(IDataReader reader, int queryHash, T obj)
        {
            if (!_mappers.ContainsKey(queryHash))
                CreateMapping(reader, queryHash);

            foreach (var val in _mappers[queryHash])
            {
                _properties[val.Name](obj, reader, val.ReaderIndex);
            }
        }

        private void CreateMapping(IDataReader reader, int queryHash)
        {
            var list = new List<FieldInfoMapper>();
            _mappers.Add(queryHash, list);

            foreach (var setter in _properties)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i).ToLower();

                    if (setter.Key.Equals(name))
                    {
                        var mapper = new FieldInfoMapper();
                        mapper.Name = name;
                        mapper.ReaderIndex = i;
                        list.Add(mapper);
                    }
                }
            }
        }

        #region Create setters

        private static string GetPropertyName(PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(false);
            if (attrs.Length > 0)
            {
                for (int i = 0; i < attrs.Length; i++)
                {
                    var attr = attrs[i] as DbField;
                    if (attr != null)
                    {
                        if (attr.ExcludeFromMapping)
                        {
                            return string.Empty;
                        }
                        else
                        {
                            return attr.Name.ToLower();
                        }
                    }
                }
            }

            return propertyInfo.Name.ToLower();
        }

        private static void AddPropertySetter(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;

            //in case of private setter it must return null
            if (propertyInfo.GetSetMethod() == null)
                return;

            //when some property is excluded from the mapping
            string propertyName = GetPropertyName(propertyInfo);
            if (string.IsNullOrEmpty(propertyName))
                return;

            if (type == typeof(string))
            {
                Action<T, string> setter = propertyInfo.InitializeSet<T, string>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetNullableStringExt(index)); });
            }
            else if (type == typeof(int))
            {
                Action<T, int> setter = propertyInfo.InitializeSet<T, int>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetInt32Ext(index)); });
            }

            else if (type == typeof(int?))
            {
                Action<T, int?> setter = propertyInfo.InitializeSet<T, int?>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetNullableIntExt(index)); });
            }
            else if (type == typeof(long))
            {
                Action<T, long> setter = propertyInfo.InitializeSet<T, long>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetLongExt(index)); });
            }
            else if (type == typeof(long?))
            {
                Action<T, long?> setter = propertyInfo.InitializeSet<T, long?>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetNullableLongExt(index)); });
            }
            else if (type == typeof(double))
            {
                Action<T, double> setter = propertyInfo.InitializeSet<T, double>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetDoubleExt(index)); });
            }
            else if (type == typeof(double?))
            {
                Action<T, double?> setter = propertyInfo.InitializeSet<T, double?>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetNullableDoubleExt(index)); });
            }
            else if (type == typeof(DateTime))
            {
                Action<T, DateTime> setter = propertyInfo.InitializeSet<T, DateTime>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetDateTimeExt(index)); });
            }
            else if (type == typeof(DateTime?))
            {
                Action<T, DateTime?> setter = propertyInfo.InitializeSet<T, DateTime?>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetNullableDateTimeExt(index)); });
            }
            else if (type == typeof(Decimal))
            {
                Action<T, Decimal> setter = propertyInfo.InitializeSet<T, Decimal>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetDecimalExt(index)); });
            }
            else if (type == typeof(bool))
            {
                Action<T, bool> setter = propertyInfo.InitializeSet<T, bool>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetBooleanExt(index)); });
            }
            else if (type == typeof(bool?))
            {
                Action<T, bool?> setter = propertyInfo.InitializeSet<T, bool?>();
                _properties.Add(propertyName, (obj, reader, index) => { setter(obj, reader.GetNullableBooleanExt(index)); });
            }
        }

        #endregion

        #region Extensions

        public void AddExtension<RetType>(Expression<Func<T, RetType>> selector, Func<IDataReader, int, RetType> func)
        {
            var ext = new MapperExtensionItem<T>();
            ext.Build(selector, func);
            AddExtension(ext);
        }

        public void AddExtension(MapperExtensionItem<T> extension)
        {
            if (extension == null || !extension.IsSet)
                throw new ArgumentNullException("extension", "Cannot be null");
            if (_properties.ContainsKey(extension.PropertyName))
                throw new MapperException(string.Format("Property '{0}' already exists", extension.PropertyName));

            _properties.Add(extension.PropertyName, extension.Action);
        }

        public void OverrideExtension(MapperExtensionItem<T> extension)
        {
            if (extension == null || !extension.IsSet)
                throw new ArgumentNullException("extension", "Cannot be null");
            if (!_properties.ContainsKey(extension.PropertyName))
                throw new MapperException(string.Format("Property '{0}' not found", extension.PropertyName));

            _properties[extension.PropertyName] = extension.Action;
        }

        public MapperExtensionItem<T> CreateExtension()
        {
            return new MapperExtensionItem<T>();
        }

        #endregion
    }
}
