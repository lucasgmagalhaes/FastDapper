using System;

namespace FastDapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public string Schema { get; set; }

        public TableAttribute(string name, string schema = null)
        {
            Name = name;
            Schema = schema;
        }
    }
}