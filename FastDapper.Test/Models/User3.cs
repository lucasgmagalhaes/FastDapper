using FastDapper.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FastDapper.Test.Models
{
    internal class User3
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
