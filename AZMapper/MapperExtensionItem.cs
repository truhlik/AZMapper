using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using AZMapper.Extensions;

namespace AZMapper
{
    public class MapperExtensionItem<T>
        where T : class, new()
    {
        public Action<T, IDataReader, int> Action { get; private set; }
        public string PropertyName { get; private set; }
        public bool IsSet { get; private set; }

        public MapperExtensionItem()
        {
            IsSet = false;
        }

        public void Build<RetType>(Expression<Func<T, RetType>> selector, Func<IDataReader, int, RetType> func)
        {
            if (IsSet)
                throw new MapperException("Extension is already set");
            IsSet = true;

            var propertyInfo = GetMemberInfo<T, RetType>(selector);
            PropertyName = propertyInfo.Name.ToLower().Trim();

            Action<T, RetType> setter = propertyInfo.InitializeSet<T, RetType>();
            Action = (obj, reader, index) => { setter(obj, func(reader, index)); };
        }

        private PropertyInfo GetMemberInfo<Q, TValue>(Expression<Func<Q, TValue>> selector)
        {
            Expression body = selector;

            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }

            if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (PropertyInfo)((MemberExpression)body).Member;
            }
            else
            {
                throw new MapperException("Field or property not found");
            }
        }
    }
}
