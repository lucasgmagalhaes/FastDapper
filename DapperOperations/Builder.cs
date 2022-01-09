using DapperOperations.Exceptions;
using DapperOperations.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace DapperOperations
{
    public static class Builder
    {
        private static readonly BuilderCache _cache = new();

        public static string BuildInsertStatement<T>() where T : class, new()
        {
            var mapper = DapperOperation.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
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

        public static string BuildUpsertStatement<T>(int count, bool update, Expression<Func<T, object>>? conflictKeys = null) where T : class, new()
        {
            var mapper = DapperOperation.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
            }

            var cache = _cache.GetUpsertCache<T>();

            if (cache is not null)
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

            _cache.AddUpsertCache<T>(cache);

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
            cache.Conflicts.Add(conflictKeys.Body, conflict);
            return conflict;
        }

        private static string BuildFromCache<T>(
            int count,
            bool update,
            Expression<Func<T, object>>? conflictKeys,
            [NotNull] MappedEntity<T> mapper,
            [NotNull] UpsertCache cache) where T : class, new()
        {
            var queryBuilder = new StringBuilder($"INSERT INTO {mapper.GetFormattedTableName()} ({cache.FieldsFormatted}) VALUES {cache.RepeatValues(count)} ON CONFLICT ");

            if (update)
            {
                var conflictCache = cache.Conflicts.FirstOrDefault(c => IsExpressionEqual(c.Key, conflictKeys));
                queryBuilder.Append(conflictCache.Value);

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

        public static string BuildUpdateStatement<T>() where T : class, new()
        {
            var mapper = DapperOperation.Get<T>();

            if (mapper == null)
            {
                throw new SqlBuildingException("Mapper not found for " + typeof(T).Name);
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
                updateQuery.Append($" WHERE {mapper.KeyMap?.FirstOrDefault().Value}=@{mapper.KeyMap.FirstOrDefault().Key}");
                return _cache.AddUpdateQuery<T>(updateQuery.ToString());
            }
            return query;
        }

        public static object UnifyEntities<T>(T[] entities)
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

        private static bool IsExpressionEqual(Expression exp1, Expression exp2)
        {
            var props1 = Utils.GetPropertiesFromExpression(exp1);
            var props2 = Utils.GetPropertiesFromExpression(exp2);

            if (props1.Length != props2.Length)
            {
                return false;
            }

            for (int i = 0; i < props1.Length; i++)
            {
                if (!props1[i].Equals(props2[i]) && !props1.Contains(props2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
