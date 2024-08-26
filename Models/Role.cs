using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string MeaningfulName { get; set; }
        public bool IsDelete { get; set; }
    }
}
