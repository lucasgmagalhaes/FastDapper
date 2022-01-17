using System.ComponentModel.DataAnnotations;

namespace FastDapper.Test.Models
{
    internal class User3
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
