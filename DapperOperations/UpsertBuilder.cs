using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperOperations
{
    public class UpsertBuilder<TEntity, TId> where TEntity : class, new()
    {
        private bool _update = true;
        private readonly string _table;
        private Expression<Func<TEntity, object>> _conflitctKeys;
        private readonly IDbConnection _con;
        private readonly IEnumerable<TEntity> _entries;

        public UpsertBuilder(IDbConnection con, IEnumerable<TEntity> entries, string table)
        {
            _con = con;
            _entries = entries;
            _table = table;
        }

        public UpsertBuilder<TEntity, TId> OnConflit(Expression<Func<TEntity, object>> conflitctKeys)
        {
            _conflitctKeys = conflitctKeys;
            return this;
        }

        public UpsertBuilder<TEntity, TId> DoNothing()
        {
            _update = false;
            return this;
        }

        public async Task RunAsync()
        {
            var sql = Builder.BuildBulkInsertStatement<TEntity>(_entries.Count());
            var obj = BuildFullObject();
            var ids = (await _con.QueryAsync<TId>(sql, obj)).ToArray();
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

        private string BuildSQLStatement()
        {
            var builder = new StringBuilder($"INSERT INTO {_table}");

            var properties = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<IgnoreAttribute>() == null && p.GetCustomAttribute<KeyAttribute>() == null)
            .ToDictionary(y => y.Name, x => GetColumnNameFromProp(x));
            var fields = string.Join(", ", properties.Select(x => $"\"{x.Value}\""));

            builder.Append($"({fields}) VALUES ");

            var values = new List<string>();
            for (int i = 0; i < _entries.Count(); i++)
            {
                values.Add($"({string.Join(", ", properties.Select(x => $"@{x.Key}_{i}"))})");
            }

            builder.Append(string.Join(',', values));

            builder.Append(" ON CONFLICT ");

            var obj = _conflitctKeys.Compile().Invoke(_entries.First());

            var propertiesConflict = obj.GetType().GetProperties()
                .Select(o => o.Name).Select(name => properties.GetValueOrDefault(name));

            builder.Append($"({string.Join(',', propertiesConflict)})");

            if (_update)
            {
                builder.Append(" DO UPDATE ");
                builder.Append($"SET ");

                var onUpdateList = new List<string>();
                foreach (var property in properties.Select(p => p.Value))
                {
                    onUpdateList.Add($"{property} = EXCLUDED.{property}");
                }
                builder.Append(string.Join(',', onUpdateList));
            }

            if (typeof(TEntity).GetProperty("Id") != null)
            {
                builder.Append(" RETURNING Id");
            }

            return builder.ToString();
        }

        private static string GetColumnNameFromProp(PropertyInfo prop)
        {
            var col = prop.GetCustomAttribute<ColumnAttribute>();

            if (col != null)
            {
                return col.Name;
            }

            return prop.Name.ToSnakeCase();
        }

    }
}
