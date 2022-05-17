using System;

namespace FastDapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ColumnAttribute : Attribute
    {
        public readonly string Name;

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
