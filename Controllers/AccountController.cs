using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Helpers;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Controllers
{
    public class AccountController : Controller
    {
        private readonly FGCDbContext context;
        private readonly UserManager<TblUser> userManager;
        private readonly EmailManager emailManager;
        private readonly IEmailSender _emailsender;
        public EmailService _emailService;
        //  private readonly ILogger<LoginModel> _logger;
        public AccountController(FGCDbContext context, EmailManager emailManager, IEmailSender _emailsender)
        {
            this.context = context;
            this.emailManager = emailManager;
            this._emailsender = _emailsender;
            this._emailService = new EmailService();

        }

        [BindProperty]
        public UserInfoViewModel Input { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword(int t, string userId, string token)
        {
            ViewBag.Message = t == 1 ? "Your email is succesfully confirmed, please create password" : "Please provide new password";

            return View(new UserPasswordViewModel() { UserId = userId, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserPasswordViewModel viewModel)
        {
            IdentityResult result = null;
            TblUser resultt = null;
            if (viewModel.UserId == null)
            {
                ModelState.AddModelError(string.Empty, "User is not found, this link is tempered");
                return View(viewModel);
            }
            if (string.IsNullOrWhiteSpace(viewModel.Password))
            {
                ModelState.AddModelError(string.Empty, "Password cannot be empty");
                return View(viewModel);
            }
            if (viewModel.Password != viewModel.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(viewModel.ConfirmPassword), "Password and Confirm Password must match");
                return View(viewModel);
            }
            //var user = await userManager.FindByIdAsync(viewModel.UserId);
            var user = context.TblUsers.Where(x => x.UserId == Convert.ToInt32(viewModel.UserId)).FirstOrDefault(); ;
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User is not valid, this link is tempered");
                return View(viewModel);
            }

            if (user.Password == null)
            {
                //result = await userManager.AddPasswordAsync(user, viewModel.Password);
                user.Password = viewModel.Password;
            }
            else
            {
                if (viewModel.Token == null)
                {
                    ModelState.AddModelError(string.Empty, "Token is not found, this link is tempered");
                    return View(viewModel);
                }
                //result = await userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
                resultt = context.TblUsers.Where(x => x.UserId == Convert.ToInt32(viewModel.UserId) && x.UserKey == viewModel.Token).FirstOrDefault();
            }

            if (resultt == null)
            {
                ModelState.AddModelError(string.Empty, string.Join("<br/>", result.Errors.Select(x => x.Description)));
                return View(viewModel);
            }
            // user.ExpireDate = Shared.MyDateTime.Today.AddDays(user.PasswordExpiryInDays);
            resultt.Password = viewModel.Password;
            await context.SaveChangesAsync();

            if (resultt == null)
            {
                ModelState.AddModelError(string.Empty, string.Join("<br/>", result.Errors.Select(x => x.Description)));
                return View(viewModel);
            }
            ViewBag.Message = "Your password is updated successfully";
            return View(new UserPasswordViewModel());
        }
        [HttpGet]
        public IActionResult UserInfo()
        {
            ViewBag.Message = TempData["msge"];
            ViewBag.type = TempData["MsgType"];
            UserInfoViewModel viewModel = new UserInfoViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UserInfo(string userName)
        {
            UserInfoViewModel viewModel = new UserInfoViewModel();
            var hh = Input.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                TempData["msge"] = "Please Provide User Name";
                return RedirectToAction("UserInfo");
            }
            //var user = await userManager.FindByNameAsync(userName);
            var user = context.TblUsers.Where(x => x.Email == userName).FirstOrDefault();
            if (user == null)
            {
                // return BadRequest("User is not found");
                TempData["msge"] = "Sorry , we could not found a user account associated with this email";
                return RedirectToAction("UserInfo");
            }
            //var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var token = user.UserKey;
            var changePasswordLink = Url.Action(nameof(ChangePassword), "Account", new { userId = user.UserId, token = token }, Request.Scheme);
            //logger.LogInformation($"change Password Link : {changePasswordLink}");
            try
            {
                var emailBody = $"This is automated email for password recovery, <a href='{changePasswordLink}'>Click here</a> to create your new password.";
                //await emailManager.SendEmail(user.Email, emailBody);
                await _emailService.SendEmailAsync(user.Email, "Rest password", emailBody, "");
            }
            catch (Exception ex)
            {
                TempData["msge"] = "Email Sending failure";
                return RedirectToAction("UserInfo");
            }
            TempData["msge"] = "Email is sent Please check your inbox";
            return RedirectToAction("UserInfo");
            //return RedirectToAction("Login", "identity/account");
        }
    }
}
