namespace DapperOperations
{
    internal class BuilderCache
    {
        private readonly Dictionary<Guid, EntityQuery> _cache = new();

        public string AddInsertQuery<T>(string queryInsert)
        {
            var key = Utils.GetTypeKey(typeof(T));
            var query = _cache[key];
            if (query == null)
            {
                _cache.Add(key, new EntityQuery { Insert = queryInsert });
                return queryInsert;
            }
            query.Insert = queryInsert;
            return queryInsert;
        }

        public string GetInsertQuery<T>()
        {
            var key = Utils.GetTypeKey(typeof(T));
            var query = _cache[key];
            if (query == null)
            {
                return "";
            }
            return query.Insert;
        }

        public string AddUpdateQuery<T>(string queryUpdate)
        {
            var key = Utils.GetTypeKey(typeof(T));
            var query = _cache[key];
            if (query == null)
            {
                _cache.Add(key, new EntityQuery { Update = queryUpdate });
                return queryUpdate;
            }
            query.Update = queryUpdate;
            return queryUpdate;
        }

        public string GetUpdateQuery<T>()
        {
            var key = Utils.GetTypeKey(typeof(T));
            var query = _cache[key];
            if (query == null)
            {
                return "";
            }
            return query.Update;
        }

        public UpsertCache AddUpsertCache<T>(UpsertCache cache)
        {
            var key = Utils.GetTypeKey(typeof(T));
            var query = _cache[key];
            if (query == null)
            {
                _cache.Add(key, new EntityQuery { Upsert = cache });
                return cache;
            }
            query.Upsert = cache;
            return cache;
        }

        public UpsertCache GetUpsertCache<T>()
        {
            var key = Utils.GetTypeKey(typeof(T));
            var query = _cache[key];
            if (query == null)
            {
                return null;
            }
            return query.Upsert;
        }
    }
}
