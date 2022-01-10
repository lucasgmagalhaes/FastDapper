using DapperOperations.Attributes;
using DapperOperations.Exceptions;
using DapperOperations.Extensions;
using DapperOperations.Mapping;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace DapperOperations
{
    /// <summary>
    /// Represents all configurations for <see cref="DapperOperation"/>
    /// </summary>
    public static class DapperOperation
    {
        /// <summary>
        /// Gets the namming convention used to map each property to his
        /// refered column
        /// </summary>
        public static NameConvetion NameConvetion
        {
            get => _nameConvetion;
        }

        /// <summary>
        /// Gets or set the definition of <see cref="DapperOperation"/> must
        /// throw an error if some entity be mapped more than once.
        /// </summary>
        public static bool ThrowIfAlreadyMapped { get; set; }

        private static readonly Hashtable _mapper = new();
        private static NameConvetion _nameConvetion = NameConvetion.CamelCase;

        internal static bool HasMappedProperties { get; set; } = false;

        /// <summary>
        /// Set the namming convention for table and columns names. <see cref="NameConvetion"/>
        /// </summary>
        /// <param name="nameConvetion">The nameConvetion to be used</param>
        /// <exception cref="MappingException">
        /// Throws an error if the attempting to change 
        /// the convetion after an entity be mapped
        /// </exception>
        public static void SetNameConvention(NameConvetion nameConvetion)
        {
            if (HasMappedProperties)
            {
                throw new MappingException("Can not change namming convention after map properties");
            }

            _nameConvetion = nameConvetion;
        }

        /// <summary>
        /// Get a specified mapped entity <typeparamref name="T"/> or null if the entity isn't mapped already
        /// </summary>
        /// <typeparam name="T">Entity mapped</typeparam>
        /// <returns>The mapper of entity <typeparamref name="T"/> or null</returns>
        public static MappedEntity<T>? Get<T>() where T : class, new()
        {
            var key = Utils.GetTypeKey<T>();
            var entity = _mapper[key];
            if (entity == null)
            {
                return null;
            }
            return (MappedEntity<T>)entity;
        }

        /// <summary>
        /// Get a no specified mapped entity or null if the entity isn't mapped
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static MappedEntity? Get(Type model)
        {
            var key = Utils.GetTypeKey(model);
            var entity = _mapper[key];
            if (entity == null)
            {
                return null;
            }
            return (MappedEntity)entity;
        }

        public static MappedEntity<T> CreateEmptyMap<T>() where T : class, new()
        {
            if (IsEntityMapped(typeof(T)) && ThrowIfAlreadyMapped)
            {
                throw new MappingException($"Entity {typeof(T).Name} is already mapped");
            }

            var mapper = new MappedEntity<T>();
            var key = Utils.GetTypeKey<T>();
            _mapper.Add(key, mapper);
            return mapper;
        }

        public static MappedEntity<T>? Map<T>() where T : class, new()
        {
            var key = Utils.GetTypeKey<T>();
            if (IsEntityMapped(typeof(T)))
            {
                ThrowErrorIfAlreadyMapped<T>();
                return (MappedEntity<T>?)_mapper[key];
            }

            var mapper = new MappedEntity<T>();
            InitMap(mapper, typeof(T));
            _mapper.Add(key, mapper);
            return mapper;
        }

        public static MappedEntity? Map(Type entity)
        {
            var key = Utils.GetTypeKey(entity);
            if (IsEntityMapped(entity))
            {
                ThrowErrorIfAlreadyMapped(entity);
                return (MappedEntity?)_mapper[key];
            }

            var mapper = new MappedEntity();
            InitMap(mapper, entity);
            _mapper.Add(key, mapper);
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
            var key = Utils.GetTypeKey(entity);

            if (IsEntityMapped(entity))
            {
                ThrowErrorIfAlreadyMapped(entity);
                _mapper.Add(key, value);
                return value;
            }
            else
            {
                InitMap(value, entity);
                _mapper.Add(key, value);
                return value;
            }
        }

        public static MappedEntity? GetOrAdd<T>() where T : class, new()
        {
            var value = new MappedEntity<T>();
            var key = Utils.GetTypeKey<T>();

            if (IsEntityMapped(typeof(T)))
            {
                _mapper.Add(key, value);
                return value;
            }
            else
            {
                InitMap(value, typeof(T));
                _mapper.Add(key, value);
                return value;
            }
        }

        public static bool IsEntityMapped<T>() where T : class, new()
        {
            return _mapper.ContainsKey(typeof(T).GUID);
        }

        public static bool IsEntityMapped(Type type)
        {
            return _mapper.ContainsKey(type.GUID);
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

        private static void ThrowErrorIfAlreadyMapped<T>()
        {
            ThrowErrorIfAlreadyMapped(typeof(T));
        }

        private static void ThrowErrorIfAlreadyMapped(Type type)
        {
            if (ThrowIfAlreadyMapped)
            {
                throw new MappingException($"Entity {type.Name} is already mapped");
            }
        }
    }
}
