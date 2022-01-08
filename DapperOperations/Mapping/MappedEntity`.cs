namespace DapperOperations.Mapping
{
    public class MappedEntity
    {
        public Tuple<string, string>? KeyMap { get; set; }
        public Dictionary<string, string> ColumnsMap { get; set; }  
        public string TableName { get; set; }
        public string? SchemaName { get; set; }

        public MappedEntity()
        {
            TableName = "";
            ColumnsMap = new Dictionary<string, string>();
        }

        public void Key(string source, string destination)
        {
            KeyMap = new(source, destination);
        }

        public void Column(string propName, string columnDestination)
        {
            ColumnsMap.Add(propName, columnDestination);
        }

        public void Table(string name)
        {
            TableName = name;
        }

        public void Table(string name, string? schema)
        {
            TableName = name;
            SchemaName = schema;
        }

        public string GetFormattedTableName()
        {
            if (string.IsNullOrEmpty(SchemaName))
            {
                return TableName;
            }
            return $"{SchemaName}.{TableName}";
        }
    }
}
