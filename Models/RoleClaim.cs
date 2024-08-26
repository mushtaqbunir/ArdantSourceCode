using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Models
{
    public class RoleClaim
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
        public bool IsDelete { get; set; }
    }
}
