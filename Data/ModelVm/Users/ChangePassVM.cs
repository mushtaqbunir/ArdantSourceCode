using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArdantOffical.Data.ModelVm.Users
{
    public class ChangePassVM
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 8 and 15 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 8 and 15 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string NewPassword { get; set; }
        [NotMapped] // Does not effect with your database
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

        public int? UserStatus { get; set; } = 1;
        public string OnlineStatus { get; set; } = "Offline";

    }
}
