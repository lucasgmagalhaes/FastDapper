using FastDapper.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace FastDapper.Mapping
{
    /// <summary>
    /// Defines a typed entity mapper
    /// For a non typed mapping, uses: <see cref="MappedEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">Entity to be mapped</typeparam>
    public class MappedEntity<TEntity> : MappedEntity where TEntity : class, new()
    {
        /// <summary>
        /// Manually maps a property of <typeparamref name="TEntity"/>
        /// 
        /// <br></br>
        /// 
        /// For the column name, <see cref="FastDapper"/> uses the <see cref="FastDapper.NameConvetion"/> to format
        /// the name.
        /// 
        /// </summary>
        /// <param name="keyMapper">Property of <typeparamref name="TEntity"/> to be mapped</param>
        public void Column([NotNull] Expression<Func<TEntity, object>> keyMapper)
        {
            var property = GetPropertyInfo(keyMapper);
            ColumnsMap.Add(property.Name, property.Name.FormatByConvetion());
        }

        /// <summary>
        /// Manually maps a property of <typeparamref name="TEntity"/> with a column name
        /// defined in <paramref name="destinationName"/>
        /// </summary>
        /// <param name="keyMapper">Property of <typeparamref name="TEntity"/> to be mapped</param>
        /// <param name="destinationName">Column that <paramref name="keyMapper"/> refers to</param>
        public void Column(Expression<Func<TEntity, object>> keyMapper, string destinationName)
        {
            var property = GetPropertyInfo(keyMapper);
            ColumnsMap.Add(property.Name, destinationName.FormatByConvetion());
        }

        /// <summary>
        /// Defines the primary key of <typeparamref name="TEntity"/>
        /// 
        /// <br></br>
        /// 
        /// For the column name, <see cref="FastDapper"/> uses the <see cref="FastDapper.NameConvetion"/> to format
        /// the name.
        /// </summary>
        /// <param name="keyMapper">Primary key property</param>
        public void PrimaryKey(Expression<Func<TEntity, object>> keyMapper)
        {
            var property = GetPropertyInfo(keyMapper);
            KeyMap.Add(property.Name, property.Name.FormatByConvetion());
        }

        /// <summary>
        /// Defines the primary key of <typeparamref name="TEntity"/> with a column name
        /// defined in <paramref name="destinationName"/>
        /// </summary>
        /// <param name="keyMapper">Property of <typeparamref name="TEntity"/> to be mapped</param>
        /// <param name="destinationName">Column that <paramref name="keyMapper"/> refers to</param>
        public void PrimaryKey(Expression<Func<TEntity, object>> keyMapper, string destinationName)
        {
            var property = GetPropertyInfo(keyMapper);
            KeyMap.Add(property.Name, destinationName.FormatByConvetion());
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
