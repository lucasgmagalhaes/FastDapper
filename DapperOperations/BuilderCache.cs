using System.Collections.Concurrent;

namespace DapperOperations
{
    internal class BuilderCache
    {
        private readonly ConcurrentDictionary<Guid, EntityQuery> _cache = new();

        public string AddInsertQuery<T>(string queryInsert)
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var query);
            if (query == null)
            {
                _cache.TryAdd(key, new EntityQuery { Insert = queryInsert });
                return queryInsert;
            }
            query.Insert = queryInsert;
            return queryInsert;
        }

        public string GetInsertQuery<T>()
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var query);
            if (query == null)
            {
                return "";
            }
            return query.Insert;
        }

        public string AddUpdateQuery<T>(string queryUpdate)
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var query);
            if (query == null)
            {
                _cache.TryAdd(key, new EntityQuery { Update = queryUpdate });
                return queryUpdate;
            }
            query.Update = queryUpdate;
            return queryUpdate;
        }

        public string GetUpdateQuery<T>()
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var query);
            if (query == null)
            {
                return "";
            }
            return query.Update;
        }

        public UpsertCache AddUpsertCache<T>(UpsertCache cache)
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var query);
            if (query == null)
            {
                _cache.TryAdd(key, new EntityQuery { Upsert = cache });
                return cache;
            }
            query.Upsert = cache;
            return cache;
        }

        public UpsertCache GetUpsertCache<T>()
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var query);
            if (query == null)
            {
                return null;
            }
            return query.Upsert;
        }
    }
}
