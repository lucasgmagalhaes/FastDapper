using Dapper;
using DapperOperations.Exceptions;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;

namespace DapperOperations
{
    public class UpsertBuilder<TEntity> where TEntity : class, new()
    {
        private bool _update = true;
        private readonly string _table;
        private Expression<Func<TEntity, object>> _conflitctKeys;
        private readonly IDbConnection _con;
        private readonly IEnumerable<TEntity> _entries;

        public UpsertBuilder(IDbConnection con, IEnumerable<TEntity> entries)
        {
            _con = con;
            _entries = entries;
            var mapper = DapperOperation.Get<TEntity>();

            if (mapper is null)
            {
                throw new MappingException(nameof(mapper));
            }

            _table = mapper.TableName;
        }

        public UpsertBuilder<TEntity> OnConflit(Expression<Func<TEntity, object>> conflitctKeys)
        {
            _conflitctKeys = conflitctKeys;
            return this;
        }

        public UpsertBuilder<TEntity> DoNothing()
        {
            _update = false;
            return this;
        }

        public async Task RunAsync()
        {
            var sql = Builder.BuildUpsertStatement<TEntity>(_entries.Count(), _update, _conflitctKeys);
            var obj = BuildFullObject();
            var ids = (await _con.QueryAsync<object>(sql, obj)).ToArray();
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

        public object BuildFullObject()
        {
            var full = new ExpandoObject() as IDictionary<string, object>;
            var entryArray = _entries.ToArray();
            for (int i = 0; i < entryArray.Length; i++)
            {
                foreach (var prop in entryArray[i].GetType().GetProperties())
                {
                    full.Add($"{prop.Name}_{i}", prop.GetValue(entryArray[i]));
                }
            }
            return full;
        }

    }
}
