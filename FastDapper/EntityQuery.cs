namespace FastDapper
{
    internal class EntityQuery
    {
        internal string Insert { get; set; }
        internal string Update { get; set; }
        internal string Delete { get; set; }

        internal QueryParameter Select { get; set; }
        internal QueryParameter SelectById { get; set; }
        internal UpsertCache Upsert { get; set; }

        internal EntityQuery()
        {
            Insert = string.Empty;
            Update = string.Empty;
        }
    }
}
