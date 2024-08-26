using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Models
{
    public class UserClaim
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int? MenuItemParentID { get; set; }
        public int UserId { get; set; }
        public int IntroducerUserId { get; set; }
        public TblUser Users { get; set; }
        public bool IsDelete { get; set; }
    }
}
