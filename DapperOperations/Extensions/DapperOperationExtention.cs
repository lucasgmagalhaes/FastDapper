using Dapper;
using System.Data;

namespace DapperOperations.Extensions
{
    public static class DapperOperationExtention
    {
        public static int Insert<T>(this IDbConnection con, T entity) where T : class, new()
        {
            return con.Execute(Builder.BuildInsertStatement<T>(), entity);
        }

        public static Task<int> InsertAsync<T>(this IDbConnection con, T entity) where T : class, new()
        {
            return con.ExecuteAsync(Builder.BuildInsertStatement<T>(), entity);
        }

        public static int Update<T>(this IDbConnection con, T entity) where T : class, new()
        {
            return con.Execute(Builder.BuildUpdateStatement<T>(), entity);
        }

        public static Task<int> UpdateAsync<T>(this IDbConnection con, T entity) where T : class, new()
        {
            return con.ExecuteAsync(Builder.BuildUpdateStatement<T>(), entity);
        }

        public static int BulkInsert<T>(this IDbConnection con, IEnumerable<T> entities) where T : class, new()
        {
            var array = entities.ToArray();
            return con.Execute(Builder.BuildBulkInsertStatement<T>(array.Length), Builder.UnifyEntities(array));
        }

        public static Task<int> BulkInsertAsync<T>(this IDbConnection con, IEnumerable<T> entities) where T : class, new()
        {
            var array = entities.ToArray();
            return con.ExecuteAsync(Builder.BuildBulkInsertStatement<T>(array.Length), Builder.UnifyEntities(array));
        }
    }
}
