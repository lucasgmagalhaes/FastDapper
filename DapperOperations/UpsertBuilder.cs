using Dapper;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;

namespace DapperOperations
{
    /// <summary>
    /// Represents a builder for upsert operations
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity that this upsert is referenced to</typeparam>
    public class UpsertBuilder<TEntity> where TEntity : class, new()
    {
        private bool _update = true;
        private Expression<Func<TEntity, object>>? _conflitctKeys;
        private readonly IDbConnection _con;
        private readonly TEntity[] _entries;

        /// <summary>
        /// Creates a new instance of this upsert builder with a given database connection and entities
        /// </summary>
        /// <param name="con">The database connection</param>
        /// <param name="entries">The entities that will be processed</param>
        public UpsertBuilder(IDbConnection con, IEnumerable<TEntity> entries)
        {
            _con = con;
            _entries = entries.ToArray();
        }

        /// <summary>
        /// Defines the values used in conflict statement.
        /// </summary>
        /// <param name="conflitctKeys">Properties of the conflict</param>
        /// <returns>This instance</returns>
        public UpsertBuilder<TEntity> OnConflit(Expression<Func<TEntity, object>> conflitctKeys)
        {
            _conflitctKeys = conflitctKeys;
            return this;
        }

        /// <summary>
        /// Defines that when conflicted, the upsert operation should do nothing.
        /// </summary>
        /// <returns>This instance</returns>
        public UpsertBuilder<TEntity> DoNothing()
        {
            _update = false;
            return this;
        }

        /// <summary>
        /// Executes the upsert operation.
        /// </summary>
        /// <returns>The task operation</returns>
        public async Task RunAsync()
        {
            var sql = Builder.BuildUpsertStatement(_entries.Count(), _update, _conflitctKeys);
            var obj = BuildFullObject(_entries);
            var ids = (await _con.QueryAsync<object>(sql, obj)).ToArray();
            UpdateKeyValues(ids);
        }

        /// <summary>
        /// Executes the upsert operation.
        /// </summary>
        public void Run()
        {
            var sql = Builder.BuildUpsertStatement(_entries.Count(), _update, _conflitctKeys);
            var obj = BuildFullObject(_entries);
            var ids = _con.Query<object>(sql, obj).ToArray();
            UpdateKeyValues(ids);
        }

        private void UpdateKeyValues(object[] ids)
        {
            var entriesArray = _entries.ToArray();

            for (int i = 0; i < entriesArray.Length; i++)
            {
                var idProp = entriesArray[i].GetType().GetProperty("Id");
                if (idProp != null)
                {
                    idProp.SetValue(entriesArray[i], ids[i]);
                }
            }
        }

        private static object BuildFullObject(TEntity[] entities)
        {
            var full = new ExpandoObject() as IDictionary<string, object>;
            for (int i = 0; i < entities.Length; i++)
            {
                foreach (var prop in entities[i].GetType().GetProperties())
                {
                    full.Add($"{prop.Name}_{i}", prop.GetValue(entities[i]));
                }
            }
            return full;
        }
    }
}
