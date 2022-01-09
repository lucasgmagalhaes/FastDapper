using Dapper;
using DapperOperations.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

    }
}
