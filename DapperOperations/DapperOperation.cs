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
    public static class DapperOperation
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

        public static MappedEntity<TEntity>? Get<TEntity>() where TEntity : class, new()
        {
            var entity = _mapper.GetValueOrDefault(typeof(TEntity).GUID);
            if (entity == null)
            {
                return null;
            }
            return (MappedEntity<TEntity>)entity;
        }

        public static MappedEntity? Get(Type model)
        {
            var entity = _mapper.GetValueOrDefault(model.GUID);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }

        public static MappedEntity<TEntity> CreateEmptyMap<TEntity>() where TEntity : class, new()
        {
            var mapper = new MappedEntity<TEntity>();
            _mapper.TryAdd(typeof(TEntity).GUID, mapper);
            return mapper;
        }

        public static MappedEntity<TEntity>? Map<TEntity>() where TEntity : class, new()
        {
            if (IsEntityMapped(typeof(TEntity)))
            {
                _mapper.TryGetValue(typeof(TEntity).GUID, out var value);
                return (MappedEntity<TEntity>?)value;
            }

            var mapper = new MappedEntity<TEntity>();
            InitMap(mapper, typeof(TEntity));
            _mapper.TryAdd(typeof(TEntity).GUID, mapper);
            return mapper;
        }

        public static MappedEntity? Map(Type entity)
        {
            if (IsEntityMapped(entity))
            {
                _mapper.TryGetValue(entity.GUID, out var value);
                return value;
            }

            var mapper = new MappedEntity();
            InitMap(mapper, entity);
            _mapper.TryAdd(entity.GUID, mapper);
            return mapper;
        }

        public static List<MappedEntity?> Map(IEnumerable<Type> entities)
        {
            var mapeds = new List<MappedEntity?>();
            foreach (var entity in entities)
            {
                mapeds.Add(Map(entity));
            }
            return mapeds;
        }

        public static List<MappedEntity?> Map(Assembly assembly, string namespaces)
        {
            var types = assembly.GetTypes().Where(a => a.Namespace == namespaces);
            return Map(types);
        }

        public static List<MappedEntity?> MapFromAssemblyName(string fullName, string typesNameSpace)
        {
            var domainAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains(fullName));
            var models = domainAssembly?.GetTypes().Where(a => a.Namespace == typesNameSpace).ToList();

            if (models != null)
            {
                return Map(models);
            }
            return Enumerable.Empty<MappedEntity?>().ToList();
        }

        public static MappedEntity? GetOrAdd(Type entity)
        {
            var value = new MappedEntity();
            if (IsEntityMapped(entity))
            {
                var mapper = _mapper.GetOrAdd(entity.GUID, value);
                return mapper;
            }
            else
            {
                InitMap(value, entity);
                _mapper.TryAdd(entity.GUID, value);
                return value;
            }
        }

        public static MappedEntity? GetOrAdd<TEntity>() where TEntity : class, new()
        {
            var value = new MappedEntity();
            if (IsEntityMapped(typeof(TEntity)))
            {
                var mapper = _mapper.GetOrAdd(typeof(TEntity).GUID, value);
                return mapper;
            }
            else
            {
                InitMap(value, typeof(TEntity));
                _mapper.TryAdd(typeof(TEntity).GUID, value);
                return value;
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
