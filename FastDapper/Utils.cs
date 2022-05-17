using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FastDapper
{
    internal static class Utils
    {
        public static PropertyInfo[] GetPropertiesFromExpression<T>(Expression<Func<T, object>> expression)
        {
            return GetPropertiesFromExpression(expression.Body);
        }

        public static PropertyInfo[] GetPropertiesFromExpression(Expression exp)
        {
            if (exp is LambdaExpression lambda)
            {
                if (lambda.Body is UnaryExpression unary)
                {
                    exp = unary.Operand;
                }
                else
                {
                    exp = lambda.Body;
                }
            }

            if (exp is NewExpression newExp)
            {
                var members = newExp.Members.ToArray();
                for (int i = 0; i < members.Length; i++)
                {
                    ThrowIfNotProperty(members[i], exp);
                }
                return members.OfType<PropertyInfo>().ToArray();
            }

            if (exp is MemberExpression member)
            {
                ThrowIfNotProperty(member.Member, exp);
                return new[] { (PropertyInfo)member.Member };
            }

            throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",
            exp.ToString()));
        }

        public static Guid GetTypeKey(Type type)
        {
            return type.GUID;
        }

        public static Guid GetTypeKey<T>()
        {
            return GetTypeKey(typeof(T));
        }

        private static void ThrowIfNotProperty(MemberInfo member, Expression expression)
        {
            if (!Is<PropertyInfo>(member))
            {
                throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a field, not a property.",
                expression.ToString()));
            }
        }

        internal static bool Is<T>(object val)
        {
            return val is T;
        }
    }
}
