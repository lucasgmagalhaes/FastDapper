using DapperOperations.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace DapperOperations.Mapping
{
    public sealed class MappedEntity<TEntity> : MappedEntity where TEntity : class, new()
    {
        public void Column(Expression<Func<TEntity, object>> keyMapper)
        {
            var property = GetPropertyInfo(keyMapper);
            ColumnsMap.Add(property.Name, property.Name.FormatByConvetion());
        }

        public void Column(Expression<Func<TEntity, object>> keyMapper, string destinationName)
        {
            var property = GetPropertyInfo(keyMapper);
            ColumnsMap.Add(property.Name, destinationName.FormatByConvetion());
        }

        public void Key(Expression<Func<TEntity, object>> keyMapper)
        {
            var property = GetPropertyInfo(keyMapper);
            KeyMap = new(property.Name, property.Name.FormatByConvetion());
        }

        public void Key(Expression<Func<TEntity, object>> keyMapper, string destinationName)
        {
            var property = GetPropertyInfo(keyMapper);
            KeyMap = new(property.Name, destinationName.FormatByConvetion());
        }

        private static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            Expression exp;
            if (expression.Body is UnaryExpression unary)
            {
                exp = unary.Operand;
            }
            else
            {
                exp = expression.Body;
            }

            if (exp is not MemberExpression member)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",
                expression.ToString()));
            }

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    expression.ToString()));

            return propInfo;
        }
    }
}
