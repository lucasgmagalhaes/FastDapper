using System;
using System.ComponentModel.DataAnnotations;

namespace DapperOperations.Test.Models
{
    internal class User1
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
