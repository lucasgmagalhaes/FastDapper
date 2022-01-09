namespace DapperOperations
{
    internal class EntityQuery
    {
        public string Insert { get; set; }
        public string Update { get; set; }
        public UpsertCache Upsert { get; set; }

        public EntityQuery()
        {
            Insert = string.Empty;
            Update = string.Empty;
        }
    }
}
