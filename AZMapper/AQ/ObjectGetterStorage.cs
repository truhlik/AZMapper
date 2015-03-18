using System;
using System.Collections.Generic;
using AZMapper.Extensions;

namespace AZMapper.AQ
{
    public class ObjectGetterStorage
    {
        private static Dictionary<Type, Dictionary<string, GetterInfo>> _getters;
        private static ObjectGetterStorage _instance = new ObjectGetterStorage();

        public static ObjectGetterStorage Instance
        {
            get { return _instance; }
        }

        private ObjectGetterStorage() { }

        static ObjectGetterStorage()
        {
            _getters = new Dictionary<Type, Dictionary<string, GetterInfo>>();
        }

        public Dictionary<string, GetterInfo> GetGetters(Type target)
        {
            if (_getters.ContainsKey(target))
                return _getters[target];

            var getters = CreateGettersCollection(target);
            _getters.Add(target, getters);

            return getters;
        }

        public Dictionary<string, GetterInfo> CreateGettersCollection(Type target)
        {
            var properties = target.GetProperties();
            var getterList = new Dictionary<string, GetterInfo>(properties.Length);
            DbFieldName attr = null;

            for (int i = 0; i < properties.Length; i++)
            {
                attr = null;
                var attrs = properties[i].GetCustomAttributes(false);
                for (int j = 0; j < attrs.Length; j++)
                {
                    if (attrs[j] is DbFieldName)
                    {
                        attr = (DbFieldName)attrs[j];
                        break;
                    }
                }

                if (attr != null && attr.ExcludeFromMapping)
                    continue;

                var info = new GetterInfo()
                {
                    Getter = properties[i].InitializeGet(target),
                    ReturnType = properties[i].PropertyType
                };

                getterList.Add((attr == null) ? properties[i].Name : attr.Name, info);
            }

            return getterList;
        }
    }
}
