using FastDapper.Exceptions;
using FastDapper.Extensions;
using FastDapper.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace FastDapper
{
    /// <summary>
    /// Builder for all SQL statements
    /// </summary>
    internal static class Builder
    {
        private static readonly Cache _cache = new();

        /// <summary>
        /// Builds insert query
        /// </summary>
        internal static string BuildInsertStatement<T>() where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var query = _cache.GetQuery<T>(QueryType.Insert);

            if (string.IsNullOrEmpty(query))
            {
                var fields = string.Join(", ", mapper.ColumnsMap.Select(x => $"\"{x.Value}\""));
                var values = string.Join(", ", mapper.ColumnsMap.Select(x => $"@{x.Key}"));
                query = $"INSERT INTO {mapper.GetFormattedTableName()} ({fields}) VALUES ({values})";
                return _cache.AddQuery<T>(query, QueryType.Insert);
            }
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="update"></param>
        /// <param name="conflictKeys"></param>
        /// <param name="useCache">Testing only</param>
        /// <returns></returns>
        /// <exception cref="SqlBuildingException"></exception>
        internal static string BuildUpsertStatement<T>(int count, bool update, Expression<Func<T, object>>? conflictKeys = null, bool useCache = true) where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var cache = _cache.GetUpsertCache<T>();

            if (cache is not null && useCache)
            {
                return BuildFromCache(count, update, conflictKeys, mapper, cache);
            }

            cache = new UpsertCache
            {
                FieldsFormatted = string.Join(", ", mapper.ColumnsMap.Select(x => $"\"{x.Key}\""))
            };

            cache.Values = mapper.ColumnsMap.Keys.ToArray();

            var builder = new StringBuilder($"INSERT INTO {mapper.GetFormattedTableName()} ({cache.FieldsFormatted}) VALUES {cache.RepeatValues(count)} ON CONFLICT ");

            if (update && conflictKeys != null)
            {
                string conflict = BuildConflictAndAddToCache(conflictKeys, mapper, cache);
                builder.Append(conflict);

                BuildSetterAndAddToCache(mapper, cache, builder);
                builder.Append(cache.Set);
            }
            else
            {
                builder.Append(" DO NOTHING ");
            }

            if (useCache)
            {
                _cache.AddUpsertCache<T>(cache);
            }

            return builder.ToString();
        }

        private static void BuildSetterAndAddToCache<T>(
            [NotNull] MappedEntity<T> mapper,
            [NotNull] UpsertCache cache,
            StringBuilder builder) where T : class, new()
        {
            builder.Append(" DO UPDATE ");
            builder.Append($"SET ");

            var onUpdateList = new string[mapper.ColumnsMap.Count];
            var values = mapper.ColumnsMap.Select(x => x.Value).ToArray();
            for (int i = 0; i < values.Length; i++)
            {
                onUpdateList[i] = $"{values[i]} = EXCLUDED.{values[i]}";
            }

            cache.Set = string.Join(',', onUpdateList);
        }

        private static string BuildConflictAndAddToCache<T>(Expression<Func<T, object>> conflictKeys, MappedEntity<T> mapper, UpsertCache cache) where T : class, new()
        {
            var conflictProps = Utils.GetPropertiesFromExpression(conflictKeys);

            string[] mappedConflictKeys = new string[conflictProps.Length];

            for (int ci = 0; ci < conflictProps.Length; ci++)
            {
                mapper.ColumnsMap.TryGetValue(conflictProps[ci].Name, out var column);

                if (!string.IsNullOrEmpty(column))
                {
                    mappedConflictKeys[ci] = column;
                }
                else
                {
                    mapper.KeyMap.TryGetValue(conflictProps[ci].Name, out var key);
                    if (!string.IsNullOrEmpty(key))
                    {
                        mappedConflictKeys[ci] = key;
                    }
                }
            }

            var conflict = $"({string.Join(',', mappedConflictKeys)})";
            cache.Conflicts.Add(conflictKeys.Body.Type.GUID, conflict);
            return conflict;
        }

        private static string BuildFromCache<T>(
            int count,
            bool update,
            Expression<Func<T, object>> conflictKeys,
            [NotNull] MappedEntity<T> mapper,
            [NotNull] UpsertCache cache) where T : class, new()
        {
            var queryBuilder = new StringBuilder($"INSERT INTO {mapper.GetFormattedTableName()} ({cache.FieldsFormatted}) VALUES {cache.RepeatValues(count)} ON CONFLICT ");

            if (update)
            {
                var conflictCache = cache.Conflicts[conflictKeys.Body.Type.GUID];
                queryBuilder.Append(conflictCache);

                queryBuilder.Append(" DO UPDATE ");
                queryBuilder.Append($"SET ");

                queryBuilder.Append(cache.Set);
            }
            else
            {
                queryBuilder.Append(" DO NOTHING");
            }
            return queryBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="SqlBuildingException"></exception>
        internal static string BuildSelectByIdQuery<T>(object id) where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            return BuildSelectQuery(id, QueryType.SelectById, mapper.ColumnsMap, mapper);
        }

        internal static string BuildDeleteByIdQuery<T>() where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var cache = _cache.GetQuery<T>(QueryType.DeleteById);

            if (cache.HasValue())
            {
                return cache;
            }

            var query = $"DELETE {mapper.GetFormattedTableName()} WHERE {mapper.GetPrimaryKeysForWhere()}";
            _cache.AddQuery<T>(query, QueryType.DeleteById);
            return query;
        }

        internal static string BuildDeleteQuery<T>(object filter) where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var cache = _cache.GetQuery<T>(QueryType.Delete, filter);

            if (cache.HasValue())
            {
                return cache;
            }

            var query = $"DELETE {mapper.GetFormattedTableName()} WHERE {mapper.GetWhereStatement(filter)}";
            _cache.AddQuery<T>(query, QueryType.Delete);
            return query;
        }

        internal static string BuildDeleteAllQuery<T>() where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var cache = _cache.GetQuery<T>(QueryType.DeleteAll);

            if (cache.HasValue())
            {
                return cache;
            }

            var query = $"DELETE {mapper.GetFormattedTableName()}";
            _cache.AddQuery<T>(query, QueryType.Delete);
            return query;
        }

        internal static string BuildTruncateQuery<T>() where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var cache = _cache.GetQuery<T>(QueryType.Truncate);

            if (cache.HasValue())
            {
                return cache;
            }

            var query = $"TRUNCATE TABLE {mapper.GetFormattedTableName()}";
            _cache.AddQuery<T>(query, QueryType.Truncate);
            return query;
        }

        internal static string BuildSelectQuery<T>(object filter) where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            return BuildSelectQuery(filter, QueryType.Select, mapper.ColumnsMap, mapper);
        }

        private static string BuildSelectQuery<T>(object filter, QueryType queryType, Dictionary<string, string> columnPairs, MappedEntity<T> mapper) where T : class, new()
        {
            var query = _cache.GetQuery<T>(queryType, filter);

            if (query == null)
            {
                var builder = new StringBuilder($"SELECT {mapper.GetColumnsForSelect()} FROM {mapper.GetFormattedTableName()} WHERE ");
                if (filter.GetType().IsPrimitive)
                {
                    foreach (var (prop, column) in columnPairs.Where(v => v.Key == filter.GetType().Name))
                    {
                        builder.Append($"{column}=@{prop},");
                    }
                }
                else
                {
                    var props = filter.GetType().GetProperties().Select(p => p.Name);
                    foreach (var (prop, column) in columnPairs.Where(v => props.Contains(v.Key)))
                    {
                        builder.Append($"{column}=@{prop},");
                    }
                }

                builder.Remove(builder.Length - 1, 1);

                query = builder.ToString();
                _cache.AddQuery<T>(query, QueryType.SelectById);
            }

            return query;
        }

        internal static string BuildSelectQuery<T>() where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var query = _cache.GetQuery<T>(QueryType.Update);

            if (query == null)
            {
                var builder = new StringBuilder($"SELECT * FROM {mapper.GetFormattedTableName()} WHERE ");
                foreach (var (prop, column) in mapper.ColumnsMap)
                {
                    builder.Append($"{column}=@{prop},");
                }

                builder.Remove(builder.Length - 1, 1);

                query = builder.ToString();
                _cache.AddQuery<T>(query, QueryType.SelectById);
            }

            return query;
        }

        /// <summary>
        /// Builds upser query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="SqlBuildingException"></exception>
        internal static string BuildUpdateStatement<T>() where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var query = _cache.GetQuery<T>(QueryType.Update);

            if (query == null)
            {
                var updateQuery = new StringBuilder($"UPDATE {mapper.GetFormattedTableName()} SET ");
                foreach (var (prop, column) in mapper.ColumnsMap)
                {
                    updateQuery.Append($"{column}=@{prop},");
                }

                updateQuery.Remove(updateQuery.Length - 1, 1);
                updateQuery.Append($" WHERE {mapper.KeyMap?.FirstOrDefault().Value}=@{mapper.KeyMap.FirstOrDefault().Key}");
                return _cache.AddQuery<T>(updateQuery.ToString(), QueryType.Update);
            }
            return query;
        }

        internal static string BuildCountQuery<T>(object? filter = null) where T : class, new()
        {
            var mapper = FastManager.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var query = _cache.GetQuery<T>(QueryType.Count, filter);

            if (query.HasValue())
            {
                return query;
            }

            if (filter != null)
            {
                query = $"SELECT COUNT(1) FROM {mapper.GetFormattedTableName()} WHERE {mapper.GetWhereStatement(filter)}";
            }
            else
            {
                query = $"SELECT COUNT(1) FROM {mapper.GetFormattedTableName()}";
            }

            _cache.AddQuery<T>(query, QueryType.Count, filter);
            return query;
        }

        /// <summary>
        /// Converts a collection of entities into a single object
        /// </summary>
        internal static object UnifyEntities<T>(T[] entities)
        {
            var full = new ExpandoObject() as IDictionary<string, object>;
            for (int i = 0; i < entities.Length; i++)
            {
                var properties = entities[i]?.GetType().GetProperties();
                for (int p = 0; p < properties?.Length; p++)
                {
                    full.Add($"{properties[p].Name}_{i}", properties[p].GetValue(entities[i]));
                }
            }
            return full;
        }
    }
}
