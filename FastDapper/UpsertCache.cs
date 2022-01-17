using System.Collections;

namespace FastDapper
{
    internal class UpsertCache
    {
        public string? FieldsFormatted { get; set; }
        public string[] Values { get; set; }
        public Hashtable Conflicts { get; set; }
        public string? Set { get; set; }

        public UpsertCache()
        {
            Conflicts = new Hashtable();
            Values = Array.Empty<string>();
        }

        public string RepeatValues(int times)
        {
            var values = new string[times];
            for (int i = 0; i < times; i++)
            {
                values[i] = $"({string.Join(", ", Values.Select(x => $"@{x}_{i}"))})";
            }

            return string.Join(",", values);
        }

    }
}
