﻿namespace FastDapper
{
    internal class QueryParameter
    {
        internal string? Query { get; set; }
        internal Guid? FilterKey { get; private set; }

        internal QueryParameter()
        {
        }

        internal QueryParameter(string query, Guid? parameterKey)
        {
            Query = query;
            FilterKey = parameterKey;
        }

        internal void SetFilterKey(object filter)
        {
            FilterKey = filter.GetType().GUID;
        }

        internal bool IsEqual(object filter)
        {
            if (FilterKey == null && filter == null)
            {
                return true;
            }

            if (FilterKey != null)
            {
                return FilterKey.Equals(filter.GetType().GUID);
            }

            return false;
        }
    }
}
