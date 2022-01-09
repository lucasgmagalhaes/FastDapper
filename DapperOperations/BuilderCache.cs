using System.Collections.Concurrent;

namespace DapperOperations
{
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

        public UpsertCache AddUpsertCache<T>(UpsertCache cache)
        {
            _cache.TryGetValue(typeof(T).GUID, out var query);
            if (query == null)
            {
                _cache.TryAdd(typeof(T).GUID, new EntityQuery { Upsert = cache });
                return cache;
            }
            query.Upsert = cache;
            return cache;
        }

        public UpsertCache GetUpsertCache<T>()
        {
            _cache.TryGetValue(typeof(T).GUID, out var query);
            if (query == null)
            {
                return null;
            }
            return query.Upsert;
        }
    }
}
