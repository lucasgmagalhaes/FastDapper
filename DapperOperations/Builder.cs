using System.Collections.Concurrent;
using System.Dynamic;
using System.Text;

namespace DapperOperations
{

    public class EntityQuery
    {
        public string Insert { get; set; }
        public string Update { get; set; }

        public EntityQuery()
        {
            Insert = string.Empty; 
            Update = string.Empty; 
        }
    }

    internal class BuilderCache
    {
        private readonly ConcurrentDictionary<Guid, EntityQuery> _cache = new();
        public string AddInsertQuery<T>(string queryInsert)
        {
            _cache.TryGetValue(typeof(T).GUID, out var query);
            if (query == null)
            {
                _cache.TryAdd(typeof(T).GUID, new EntityQuery { Insert = queryInsert });
                return queryInsert;
            }
            query.Insert = queryInsert;
            return queryInsert;
        }

        public string GetInsertQuery<T>()
        {
            _cache.TryGetValue(typeof(T).GUID, out var query);
            if (query == null)
            {
                return "";
            }
            return query.Insert;
        }

        public string AddUpdateQuery<T>(string queryUpdate)
        {
            _cache.TryGetValue(typeof(T).GUID, out var query);
            if (query == null)
            {
                _cache.TryAdd(typeof(T).GUID, new EntityQuery { Update = queryUpdate });
                return queryUpdate;
            }
            query.Update = queryUpdate;
            return queryUpdate;
        }

        public string GetUpdateQuery<T>()
        {
            _cache.TryGetValue(typeof(T).GUID, out var query);
            if (query == null)
            {
                return "";
            }
            return query.Update;
        }
    }

    public static class Builder
    {
        private static readonly BuilderCache _cache = new();

        public static string BuildInsertStatement<T>() where T : class, new()
        {
            var mapper = DapperOperation.Get<T>();

            if (mapper == null)
            {
                throw new Exception("Mapper not found for " + typeof(T).Name);
            }

            string query = _cache.GetInsertQuery<T>();

            if (string.IsNullOrEmpty(query))
            {
                var fields = string.Join(", ", mapper.ColumnsMap.Select(x => $"\"{x.Value}\""));
                var values = string.Join(", ", mapper.ColumnsMap.Select(x => $"@{x.Key}"));
                query = $"INSERT INTO {mapper.GetFormattedTableName()} ({fields}) VALUES ({values})";
                return _cache.AddInsertQuery<T>(query);
            }
            return query;
        }

        public static string BuildBulkInsertStatement<T>(int count) where T : class, new()
        {
            var mapper = DapperOperation.Get<T>();

            if (mapper == null)
            {
                throw new Exception("Mapper not found for " + typeof(T).Name);
            }

            var fields = string.Join(", ", mapper.ColumnsMap.Select(x => $"\"{x.Key}\""));

            var valuesList = new List<string>();
            for (int i = 0; i < count; i++)
            {
                valuesList.Add("(" + string.Join(", ", mapper.ColumnsMap.Select(x => $"@{x.Key}_{i}")) + ")");
            }

            return $"INSERT INTO {mapper.GetFormattedTableName()} ({fields}) VALUES {string.Join(',', valuesList)}";
        }

        public static string BuildUpdateStatement<T>() where T : class, new()
        {
            var mapper = DapperOperation.Get<T>();

            if (mapper == null)
            {
                throw new Exception("Mapper not found for " + typeof(T).Name);
            }

            string query = _cache.GetUpdateQuery<T>();

            if (string.IsNullOrEmpty(query))
            {
                var updateQuery = new StringBuilder($"UPDATE {mapper.GetFormattedTableName()} SET ");
                foreach (var (prop, column) in mapper.ColumnsMap)
                {
                    updateQuery.Append($"{column}=@{prop},");
                }

                updateQuery.Remove(updateQuery.Length - 1, 1);
                updateQuery.Append($" WHERE {mapper.KeyMap?.Item2}=@{mapper.KeyMap?.Item1}");
                return _cache.AddUpdateQuery<T>(updateQuery.ToString());
            }
            return query;
        }

        public static object UnifyEntities<T>(T[] entities)
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
