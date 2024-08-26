using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.Users
{
    public class EditVM
    {
        public int UserID { get; set; }
        public string UserKey { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Short Name is required")]
        public string ShortName { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).{8,15}$", ErrorMessage = "Password must be between 8 and 15 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 8 and 15 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User Role is required")]
        public string UserRole { get; set; }
        public int RoleID { get; set; }
        public int? UserStatus { get; set; } = 1;
        public string OnlineStatus { get; set; } = "Offline";
        public string SalesforceID { get; set; }

    }
}
