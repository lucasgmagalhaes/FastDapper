using Dapper;
using DapperOperations.Exceptions;
using System.Data;

namespace DapperOperations.Extensions
{
    /// <summary>
    /// Represents all extension CRUD operations for <see cref="Dapper"/>
    /// </summary>
    public static class DapperOperationExtention
    {
        /// <summary>
        /// Persists <b>synchronously</b> an entity <typeparamref name="T"/> into database.
        /// 
        /// <br></br>
        /// 
        /// This operation creates a cache for the INSERT query of the entity, allowing
        /// the operation to be executed faster in future calls.
        /// 
        /// <br></br>
        /// <br></br>
        /// 
        /// Calls <see cref="SqlMapper.Execute(IDbConnection, string, object, IDbTransaction, int?, CommandType?)"/> to execute the operation.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the entity to be persisted</typeparam>
        /// <param name="con">The database connection</param>
        /// <param name="entity">The entity to be persisted</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <returns>The number of Rows affected"/></returns>
        public static int Insert<T>(this IDbConnection con, T entity, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildInsertStatement<T>();
            try
            {
                return con.Execute(query, entity, transaction, commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        /// <summary>
        /// Persists <b>asynchronously</b> an entity <typeparamref name="T"/> into database.
        /// 
        /// <br></br>
        /// 
        /// This operation creates a cache for the INSERT query of the entity, allowing
        /// the operation to be executed faster in future calls.
        /// 
        /// <br></br>
        /// <br></br>
        /// 
        /// Calls <see cref="SqlMapper.ExecuteAsync(IDbConnection, string, object, IDbTransaction, int?, CommandType?)"/> to execute the operation.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the entity to be persisted</typeparam>
        /// <param name="con">The database connection</param>
        /// <param name="entity">The entity to be persisted</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <returns>The number of Rows affected"/></returns>
        public static async Task<int> InsertAsync<T>(this IDbConnection con, T entity, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildInsertStatement<T>();
            try
            {
                return await con.ExecuteAsync(query, entity, transaction, commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
           
        }

        /// <summary>
        /// Updates <b>synchronously</b> an entity <typeparamref name="T"/> into database.
        /// 
        /// <br></br>
        /// 
        /// This operation creates a cache for the UPDATE query of the entity, allowing
        /// the operation to be executed faster in future calls.
        /// 
        /// <br></br>
        /// <br></br>
        /// 
        /// Calls <see cref="SqlMapper.ExecuteAsync(IDbConnection, string, object, IDbTransaction, int?, CommandType?)"/> to execute the operation.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the entity to be persisted</typeparam>
        /// <param name="con">The database connection</param>
        /// <param name="entity">The entity to be persisted</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <returns>The number of Rows affected"/></returns>
        public static int Update<T>(this IDbConnection con, T entity, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildUpdateStatement<T>();
            try
            {
                return con.Execute(query, entity, transaction, commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        /// <summary>
        /// Updates <b>asynchronously</b> an entity <typeparamref name="T"/> into database.
        /// 
        /// <br></br>
        /// 
        /// This operation creates a cache for the UPDATE query of the entity, allowing
        /// the operation to be executed faster in future calls.
        /// 
        /// <br></br>
        /// <br></br>
        /// 
        /// Calls <see cref="SqlMapper.ExecuteAsync(IDbConnection, string, object, IDbTransaction, int?, CommandType?)"/> to execute the operation.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the entity to be persisted</typeparam>
        /// <param name="con">The database connection</param>
        /// <param name="entity">The entity to be persisted</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <returns>The number of Rows affected"/></returns>
        public static async Task<int> UpdateAsync<T>(this IDbConnection con, T entity, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildUpdateStatement<T>();
            try
            {
                return await con.ExecuteAsync(query, entity, transaction, commandTimeout).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        /// <summary>
        /// Creates a Upsert builder to Update or Insert an given entity <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of the entity to be persisted</typeparam>
        /// <param name="con">The database connection</param>
        /// <param name="entities">Entities to be persisted or updated</param>
        /// <returns>A new instance of <see cref="UpsertBuilder{TEntity}"/></returns>
        public static UpsertBuilder<T> Upsert<T>(this IDbConnection con, IEnumerable<T> entities) where T : class, new()
        {
            return new UpsertBuilder<T>(con, entities);
        }

        private static QueryException BuildExecutionException(string query, Exception ex)
        {
            return new QueryException($"Error running query: {query}. See inner exception for detail", ex);
        }
    }
}
