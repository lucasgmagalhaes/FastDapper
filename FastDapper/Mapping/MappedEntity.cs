using FastDapper.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastDapper.Mapping
{
    /// <summary>
    /// Defines a not typed entity mapping.
    /// For a typed mapping, uses: <see cref="MappedEntity{TEntity}"/>
    /// </summary>
    public class MappedEntity
    {
        internal Dictionary<string, string> KeyMap { get; set; }
        internal Dictionary<string, string> ColumnsMap { get; set; }
        internal string TableName { get; set; }
        internal string SchemaName { get; set; }

        /// <summary>
        /// Initialize the entity mappings
        /// </summary>
        public MappedEntity()
        {
            TableName = "";
            ColumnsMap = new Dictionary<string, string>();
            KeyMap = new Dictionary<string, string>();
        }

        /// <summary>
        /// Maps the primary key of the entity.
        /// </summary>
        /// <param name="property">Object's property with the primery key</param>
        /// <param name="column">Column in database. (This operation do not apply the naming specification</param>
        /// <exception cref="ArgumentNullException">If <paramref name="property"/> or <paramref name="column"/> be null or empty</exception>
        public void PrimaryKey(string property, string column)
        {
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentNullException(nameof(column));
            }

            KeyMap.Add(property, column);
        }

        /// <summary>
        /// Maps a propety of an entity into it's referenced column in database.      
        /// </summary>
        /// <param name="property">Entity's property</param>
        /// <param name="column">Entity's table column</param>
        /// <exception cref="ArgumentNullException">If <paramref name="property"/> or <paramref name="column"/> be null or empty</exception>
        public void Column(string property, string column)
        {
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentNullException(nameof(column));
            }

            ColumnsMap.Add(property, column);
        }

        /// <summary>
        /// Maps an entity's table name
        /// </summary>
        /// <param name="name">Name of the table</param>
        /// <param name="schema">Schame of the table</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is null or empty</exception>
        public void Table(string name, string schema = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            TableName = name;
            SchemaName = schema;
        }

        internal string GetColumnsForSelect()
        {
            var sb = new StringBuilder();
            foreach (var propColumn in ColumnsMap)
            {
                sb.Append($"{propColumn.Value} as {propColumn.Key}");
            }
            return sb.ToString();
        }

        internal string GetPrimaryKeysForWhere()
        {
            var sb = new StringBuilder();
            foreach (var propColumn in KeyMap)
            {
                sb.Append($"{propColumn.Value} = @{propColumn.Key}");
            }
            return sb.ToString();
        }

        internal string GetWhereStatement(object filter)
        {
            var props = filter.GetType().GetProperties();
            var _params = new string[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                var colName = GetColumnName(props[i].Name);
                _params[i] = $"{colName} = @{props[i].Name}";
            }
            return string.Join(" and ", _params);
        }

        private string GetColumnName(string key)
        {
            return KeyMap[key] ?? ColumnsMap[key] ?? key;
        }

        internal string GetFormattedTableName()
        {
            if (string.IsNullOrEmpty(SchemaName))
            {
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new MappingException("Table name not informed");
                }
                return TableName;
            }
            return $"{SchemaName}.{TableName}";
        }
    }
}
