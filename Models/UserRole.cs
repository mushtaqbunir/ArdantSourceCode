using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public TblUser User { get; set; }
        public Role Role { get; set; }
        public bool IsDelete { get; set; }
    }
}
