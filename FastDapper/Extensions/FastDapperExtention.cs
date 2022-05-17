using Dapper;
using FastDapper.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FastDapper.Extensions
{
    /// <summary>
    /// Represents all extension CRUD operations for <see cref="Dapper"/>
    /// </summary>
    public static class FastDapperExtention
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
        public static int Insert<T>(this IDbConnection con, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
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
        public static async Task<int> InsertAsync<T>(this IDbConnection con, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
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
        public static int Update<T>(this IDbConnection con, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
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
        public static async Task<int> UpdateAsync<T>(this IDbConnection con, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con">The database connection</param>
        /// <param name="id">Id of the entity to be persisted</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <returns></returns>
        public static T GetById<T>(this IDbConnection con, object id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildSelectByIdQuery<T>(id);
            try
            {
                return con.QueryFirstOrDefault<T>(query, id, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static async Task<T> GetByIdAsync<T>(this IDbConnection con, object id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildSelectByIdQuery<T>(id);

            try
            {
                return await con.QueryFirstOrDefaultAsync<T>(query, id, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static IEnumerable<T> Get<T>(this IDbConnection con, object filter, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildSelectQuery<T>(filter);
            try
            {
                return con.Query<T>(query, filter, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static async Task<IEnumerable<T>> GetAsync<T>(this IDbConnection con, object filter, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildSelectQuery<T>(filter);
            try
            {
                return await con.QueryAsync<T>(query, filter, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static void DeleteById<T>(this IDbConnection con, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildDeleteByIdQuery<T>();
            try
            {
                con.Execute(query, entity, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static async Task DeleteByIdAsync<T>(this IDbConnection con, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildDeleteByIdQuery<T>();
            try
            {
                await con.ExecuteAsync(query, entity, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static void Delete<T>(this IDbConnection con, object entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildDeleteQuery<T>(entity);
            try
            {
                con.Execute(query, entity, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static async Task DeleteAsync<T>(this IDbConnection con, object entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildDeleteQuery<T>(entity);
            try
            {
                await con.ExecuteAsync(query, entity, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static void DeleteAll<T>(this IDbConnection con, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildDeleteAllQuery<T>();
            try
            {
                con.Execute(query, transaction: transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static async Task DeleteAllAsync<T>(this IDbConnection con, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildDeleteAllQuery<T>();
            try
            {
                await con.ExecuteAsync(query, transaction: transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static void Truncate<T>(this IDbConnection con, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildTruncateQuery<T>();
            try
            {
                con.Execute(query, transaction: transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static async Task TruncateAsync<T>(this IDbConnection con, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildTruncateQuery<T>();
            try
            {
                await con.ExecuteAsync(query, transaction: transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static int Count<T>(this IDbConnection con, object filter, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildCountQuery<T>(filter);
            try
            {
                return con.QueryFirstOrDefault<int>(query, filter, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        public static async Task<int> CountAsync<T>(this IDbConnection con, object filter, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var query = Builder.BuildCountQuery<T>(filter);
            try
            {
                return await con.QueryFirstOrDefaultAsync<int>(query, filter, transaction, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                throw BuildExecutionException(query, ex);
            }
        }

        private static QueryException BuildExecutionException(string query, Exception ex)
        {
            return new QueryException($"Error running query: {query}. See inner exception for detail", ex);
        }
    }
}
