using FastDapper.Attributes;
using FastDapper.Exceptions;
using FastDapper.Extensions;
using FastDapper.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FastDapper
{
    /// <summary>
    /// Represents all configurations for <see cref="FastDapper"/>
    /// </summary>
    public static class FastManager
    {
        /// <summary>
        /// Gets the namming convention used to map each property to his
        /// refered column
        /// </summary>
        public static NamingConvetion NameConvetion => _nameConvetion;

        /// <summary>
        /// Gets or set the definition of <see cref="FastDapper"/> must
        /// throw an error if some entity be mapped more than once.
        /// </summary>
        public static bool ThrowIfAlreadyMapped { get; set; }

        private static readonly Dictionary<Guid, MappedEntity> _mapper = new Dictionary<Guid, MappedEntity>();
        private static NamingConvetion _nameConvetion = NamingConvetion.CamelCase;

        internal static bool HasMappedProperties { get; set; } = false;

        /// <summary>
        /// Set the namming convention for table and columns names. <see cref="NameConvetion"/>
        /// </summary>
        /// <param name="nameConvetion">The nameConvetion to be used</param>
        /// <exception cref="MappingException">
        /// Throws an error if the attempting to change 
        /// the convetion after an entity be mapped
        /// </exception>
        public static void SetNameConvention(NamingConvetion nameConvetion)
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
        public static MappedEntity<T> Get<T>() where T : class, new()
        {
            var key = Utils.GetTypeKey<T>();
            _mapper.TryGetValue(key, out var entity);
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
        public static MappedEntity Get(Type model)
        {
            var key = Utils.GetTypeKey(model);
            _mapper.TryGetValue(key, out var entity);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }

        /// <summary>
        /// Instantiate an empty mapper without for a given type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Entity to be mapped</typeparam>
        /// <returns>The mapper of the entity</returns>
        /// <exception cref="MappingException">Throws if the entity is already mapped</exception>
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

        /// <summary>
        /// Maps an entity <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Entity to be mapped</typeparam>
        /// <returns>The mapper built</returns>
        /// <exception cref="MappingException">Throws if the entity is already mapped</exception>
        public static MappedEntity<T> Map<T>() where T : class, new()
        {
            var key = Utils.GetTypeKey<T>();
            if (IsEntityMapped(typeof(T)))
            {
                ThrowErrorIfAlreadyMapped<T>();
                return (MappedEntity<T>)_mapper[key];
            }

            var mapper = new MappedEntity<T>();
            InitMap(mapper, typeof(T));
            _mapper.Add(key, mapper);
            return mapper;
        }
        /// <summary>
        /// Maps an entity for <paramref name="entity"/>
        /// </summary>
        /// <param name="entity">Entity to be mapped</param>
        /// <returns>The mapper built</returns>
        /// <exception cref="MappingException">Throws if the entity is already mapped</exception>
        public static MappedEntity Map(Type entity)
        {
            var key = Utils.GetTypeKey(entity);
            if (IsEntityMapped(entity))
            {
                ThrowErrorIfAlreadyMapped(entity);
                return _mapper[key];
            }

            var mapper = new MappedEntity();
            InitMap(mapper, entity);
            _mapper.Add(key, mapper);
            return mapper;
        }

        /// <summary>
        /// Maps each element of <paramref name="entities"/>
        /// </summary>
        /// <param name="entities">Elements's type to be mapped</param>
        /// <returns>Mapper buit for each element</returns>
        /// <exception cref="MappingException">Throws if the entity is already mapped</exception>
        public static List<MappedEntity> Map(IEnumerable<Type> entities)
        {
            var mapeds = new List<MappedEntity>();
            foreach (var entity in entities)
            {
                mapeds.Add(Map(entity));
            }
            return mapeds;
        }

        /// <summary>
        /// Maps all entities of <paramref name="assembly"/> that are in <paramref name="namespaces"/>
        /// </summary>
        /// <param name="assembly">Assembly that references to all entities</param>
        /// <param name="namespaces">Namespace to filter entities</param>
        /// <returns>Collection of all mapped entities</returns>
        /// <exception cref="MappingException">Throws if the entity is already mapped</exception>
        public static List<MappedEntity> Map(Assembly assembly, string namespaces)
        {
            var types = assembly.GetTypes().Where(a => a.Namespace == namespaces);
            return Map(types);
        }

        /// <summary>
        /// Maps all entities from a given <see cref="Assembly.FullName"/> and <see cref="Type.Namespace"/>
        /// <br></br>
        /// This function search in all assemblies locatted in <see cref="AppDomain"/>
        /// </summary>
        /// <param name="fullName">Full name of the <see cref="Assembly"/></param>
        /// <param name="typeNameSpace">Namespace of the <see cref="Type"/></param>
        /// <returns>Mapper buit for each element</returns>
        /// <exception cref="MappingException">Throws if the entity is already mapped</exception>
        public static List<MappedEntity> MapFromAssemblyName(string fullName, string typeNameSpace)
        {
            var domainAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => !string.IsNullOrEmpty(a.FullName) && a.FullName.Contains(fullName));
            var models = domainAssembly?.GetTypes().Where(a => a.Namespace == typeNameSpace).ToList();

            if (models != null)
            {
                return Map(models);
            }
            return Enumerable.Empty<MappedEntity>().ToList();
        }

        /// <summary>
        /// Gets the mapper of the entity or adds it if don't
        /// </summary>
        /// <param name="entity">Type of the entity to be mapped</param>
        /// <returns>New entity mapped or the mapper that is already added</returns>
        public static MappedEntity GetOrAdd(Type entity)
        {
            var key = Utils.GetTypeKey(entity);

            _mapper.TryGetValue(key, out var value);

            if (value != null)
            {
                return value;
            }

            value = new MappedEntity();

            InitMap(value, entity);
            _mapper.Add(key, value);
            return value;
        }

        /// <summary>
        /// Gets the mapper of <typeparamref name="T"/> or adds it if don't
        /// </summary>
        /// <typeparam name="T">Type of the entity to be mapped</typeparam>
        /// <returns>New entity mapped or the mapper that is already added</returns>
        public static MappedEntity<T> GetOrAdd<T>() where T : class, new()
        {
            var key = Utils.GetTypeKey(typeof(T));

            if (_mapper.TryGetValue(key, out var value))
            {
                return (MappedEntity<T>)value;
            }

            value = new MappedEntity<T>();

            InitMap(value, typeof(T));
            _mapper.Add(key, value);
            return (MappedEntity<T>)value;
        }

        /// <summary>
        /// Checks if a mapper for <typeparamref name="T"/> already exists
        /// </summary>
        /// <typeparam name="T">Entity to check if is already mapped</typeparam>
        /// <returns>true if is already mapped. Or false if isn't</returns>
        public static bool IsEntityMapped<T>() where T : class, new()
        {
            return IsEntityMapped(typeof(T));
        }

        /// <summary>
        /// Checks if a mapper for an entity <paramref name="type"/> already exists
        /// </summary>
        /// <param name="type">Entity to check if is already mapped</param>
        /// <returns>true if is already mapped. Or false if isn't</returns>
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
            string schemaName = null;

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
                if (prop.GetCustomAttributes(typeof(PrimaryKeyAttribute)).Any())
                {
                    mapper.PrimaryKey(prop.Name, prop.Name.FormatByConvetion());
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
