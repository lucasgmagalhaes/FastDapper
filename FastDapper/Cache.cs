using System;
using System.Collections.Generic;

namespace FastDapper
{
    internal class Cache
    {
        private readonly Dictionary<Guid, EntityQuery> _cache = new Dictionary<Guid, EntityQuery>();

        public string AddQuery<T>(string query, QueryType queryType, object filter = null)
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var queryCache);
            if (queryCache == null)
            {
                queryCache = new EntityQuery();
                _cache.Add(key, queryCache);
            }

            if (queryType == QueryType.Insert)
            {
                queryCache.Insert = query;
            }

            else if (queryType == QueryType.Update)
            {
                queryCache.Update = query;
            }

            if (queryType == QueryType.Delete)
            {
                queryCache.Delete = query;
            }

            if (queryType == QueryType.Select)
            {
                if (queryCache.Select == null)
                {
                    queryCache.Select = new QueryParameter(query, filter.GetType().GUID);
                }
                else
                {
                    queryCache.Select.Query = query;
                    queryCache.Select.SetFilterKey(filter);
                }
            }

            if (queryType == QueryType.SelectById)
            {
                if (queryCache.SelectById == null)
                {
                    queryCache.SelectById = new QueryParameter(query, filter.GetType().GUID);
                }
                else
                {
                    queryCache.SelectById.Query = query;
                    queryCache.SelectById.SetFilterKey(filter);
                }
            }

            return query;
        }

        public string GetQuery<T>(QueryType queryType, object filter = null)
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var queryCache);

            if (queryCache == null)
            {
                return null;
            }

            if (queryType == QueryType.Insert)
            {
                return queryCache.Insert;
            }

            if (queryType == QueryType.Update)
            {
                return queryCache.Update;
            }

            if (queryType == QueryType.Delete)
            {
                return queryCache.Delete;
            }

            if (queryType == QueryType.Select && queryCache.Select != null && queryCache.Select.IsEqual(filter))
            {
                return queryCache.Select.Query;
            }

            if (queryType == QueryType.SelectById && queryCache.SelectById != null && queryCache.SelectById.IsEqual(filter))
            {
                return queryCache.SelectById.Query;
            }

            return null;
        }

        public UpsertCache AddUpsertCache<T>(UpsertCache cache)
        {
            var key = Utils.GetTypeKey(typeof(T));
            _cache.TryGetValue(key, out var query);
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
            _cache.TryGetValue(key, out var query);
            if (query == null)
            {
                return null;
            }
            return query.Upsert;
        }
    }
}
