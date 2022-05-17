using FastDapper.Attributes;
using System;

namespace FastDapper.Test.Models
{
    internal class User2
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public DateTime CreatedDate { get; set; }

        [Ignore]
        public object? Items { get; set; }
    }
}
