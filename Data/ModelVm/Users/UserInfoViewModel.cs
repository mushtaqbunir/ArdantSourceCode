using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.Users
{
    public class UserInfoViewModel
    {
        [BindProperty]
        [Required]
        public string UserName { get; set; }

    }
    public class UserPasswordViewModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
