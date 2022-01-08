using DapperOperations.Attributes;
using DapperOperations.Exceptions;
using DapperOperations.Extensions;
using DapperOperations.Mapping;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace DapperOperations
{
    public static class Manager
    {
        public static NameConvetion NameConvetion
        {
            get => _nameConvetion;
            set
            {
                if (HasMappedProperties)
                {
                    throw new MappingException("Can not change namming convention after map properties");
                }

                _nameConvetion = value;
            }
        }

        private static readonly ConcurrentDictionary<Guid, MappedEntity> _mapper = new();
        private static NameConvetion _nameConvetion = NameConvetion.CamelCase;

        internal static bool HasMappedProperties { get; set; } = false;

        public static MappedEntity<TEntity>? GetEntity<TEntity>() where TEntity : class, new()
        {
            var entity = _mapper.GetValueOrDefault(typeof(TEntity).GUID);
            if (entity == null)
            {
                return null;
            }
            return (MappedEntity<TEntity>)entity;
        }

        public static MappedEntity? GetEntity(Type model)
        {
            var entity = _mapper.GetValueOrDefault(model.GUID);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }

        public static MappedEntity? Map(Type entity)
        {
            var value = new MappedEntity();
            var added = _mapper.TryAdd(entity.GUID, value);
            if (added)
            {
                return value;
            }
            return null;
        }

        public static MappedEntity<TEntity>? Map<TEntity>() where TEntity : class, new()
        {
            var mapper = new MappedEntity<TEntity>();
            var added = _mapper.TryAdd(typeof(TEntity).GUID, mapper);

            if (added)
            {
                InitMap(mapper, typeof(TEntity));
            }
            return mapper;
        }

        public static MappedEntity? GetOrAdd(Type entity)
        {
            var value = new MappedEntity();
            var mapper = _mapper.GetOrAdd(entity.GUID, value);
            if (mapper != null)
            {
                InitMap(mapper, entity);
            }
            return mapper;
        }

        public static MappedEntity? MapOrAdd<TEntity>() where TEntity : class, new()
        {
            var value = new MappedEntity();
            var mapper = _mapper.GetOrAdd(typeof(TEntity).GUID, value);
            if (mapper != null)
            {
                InitMap(mapper, typeof(TEntity));
            }
            return mapper;
        }

        public static void Map(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                InitMap(type);
            }
        }

        public static bool IsEntityMapped<TEntity>() where TEntity : class, new()
        {
            return _mapper.Any(v => v.Key == typeof(TEntity).GUID);
        }

        public static bool IsEntityMapped(Type type)
        {
            return _mapper.Any(v => v.Key == type.GUID);
        }

        private static void InitMap(Type model)
        {
            var mapper = GetOrAdd(model);
            if (mapper != null)
            {
                InitMap(mapper, model);
            }
        }

        private static void InitMap(MappedEntity mapper, Type model)
        {
            if (mapper == null)
            {
                return;
            }

            var tableAttribute = model.GetCustomAttribute<TableAttribute>();

            string tableName;
            string? schemaName = null;
            if (tableAttribute != null)
            {
                tableName = tableAttribute.Name;
                schemaName = tableAttribute.Schema;
            }
            else
            {
                tableName = model.Name.FormatByConvetion();
            }

            if (IsEntityMapped(model))
            {
                return;
            }

            mapper.Table(tableName, schemaName);

            var props = model.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                if (prop.GetCustomAttributes(typeof(IgnoreAttribute)).Any())
                {
                    continue;
                }
                if (prop.GetCustomAttributes(typeof(KeyAttribute)).Any())
                {
                    mapper.Key(prop.Name, prop.Name.FormatByConvetion());
                }
                else if (prop.GetCustomAttributes(typeof(ColumnAttribute)).Any())
                {
                    var columnAttribute = prop.GetCustomAttributes(typeof(ColumnAttribute))?.FirstOrDefault();
                    if (columnAttribute is ColumnAttribute attribute)
                    {
                        mapper.Column(prop.Name, attribute.Name ?? prop.Name.FormatByConvetion());
                    }
                }
                else
                {
                    mapper.Column(prop.Name, prop.Name.FormatByConvetion());
                }
            }
        }
    }
}
