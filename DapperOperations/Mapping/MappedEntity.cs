using DapperOperations.Exceptions;

namespace DapperOperations.Mapping
{
    public class MappedEntity
    {
        internal Dictionary<string, string> KeyMap { get; set; }
        internal Dictionary<string, string> ColumnsMap { get; set; }
        internal string TableName { get; set; }
        internal string? SchemaName { get; set; }

        public MappedEntity()
        {
            TableName = "";
            ColumnsMap = new Dictionary<string, string>();
            KeyMap = new Dictionary<string, string>();
        }

        public void Key(string source, string destination)
        {
            KeyMap.Add(source, destination);
        }

        public void Column(string propName, string columnDestination)
        {
            ColumnsMap.Add(propName, columnDestination);
        }

        public void Table(string name)
        {
            TableName = name;
        }

        public void Table(string name, string schema)
        {
            TableName = name;
            SchemaName = schema;
        }

        public string GetFormattedTableName()
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
