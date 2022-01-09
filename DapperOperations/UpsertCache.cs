using System.Linq.Expressions;

namespace DapperOperations
{
    internal class UpsertCache
    {
        public string FieldsFormatted { get; set; }
        public string[] Values { get; set; }
        public Dictionary<Expression, string> Conflicts { get; set; }
        public string Set { get; set; }

        public UpsertCache()
        {
            Conflicts = new Dictionary<Expression, string>();
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
