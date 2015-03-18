using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AZMapper.Extensions
{
    public static class PropertyExtension
    {
        public static Action<TContainer, TVal> InitializeSet<TContainer, TVal>(this PropertyInfo propertyInfo)
        {
            ParameterExpression instance = Expression.Parameter(typeof(TContainer), "instance");
            ParameterExpression parameter = Expression.Parameter(propertyInfo.PropertyType, "param");

            return Expression.Lambda<Action<TContainer, TVal>>(
                Expression.Call(instance, propertyInfo.GetSetMethod(), parameter),
                new ParameterExpression[] { instance, parameter }).Compile();
        }

        public static Func<object, object> InitializeGet(this PropertyInfo info, Type type)
        {
            ParameterExpression target = Expression.Parameter(typeof(object), "target");

            return
                Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.MakeMemberAccess(
                            Expression.Convert(target, type),
                            info
                        ),
                        typeof(object)
                    ),
                    target
                ).Compile();
        }
    }
}
