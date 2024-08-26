using ArdantOffical.Data;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArdantOffical.Areas.Identity.Pages.Account
{
    public class LoginGoogleAuthenticationModel : PageModel
    {
        private readonly FGCDbContext context;


        private readonly ILogger<LoginGoogleAuthenticationModel> _logger;
        public IUsersServices IuserService;

        public LoginGoogleAuthenticationModel(ILogger<LoginGoogleAuthenticationModel> logger, FGCDbContext context, IUsersServices _IuserService)
        {


            this.context = context;
            this.IuserService = _IuserService;
            _logger = logger;

        }

        [BindProperty]
        public InputverificationModel Input { get; set; } = new();

        public string QRbase64string { get; set; } = string.Empty;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputverificationModel
        {

            public string Email { get; set; }
            [StringLength(6)]
            [Required]
            public string verificationCode { get; set; }

            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string Data, string returnUrl = null)
        {

            if (!string.IsNullOrEmpty(Data))
            {
                var DataSplit = Data.Split(',');
                var Email = DataSplit[0];
                var RememberMe = Convert.ToBoolean(DataSplit[1]);
                Input.Email = Email;
                Input.RememberMe = RememberMe;
            }

            QRbase64string = await IuserService.GoogleAuthenticator(Input.Email);


            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl = returnUrl ?? Url.Content("~/");
            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = "/";
            if (ModelState.IsValid)
            {
#if DEBUG
                TblUser checkUserr = context.TblUsers.Where(x => x.Username == Input.Email).FirstOrDefault();
                if (checkUserr != null)
                {
                    try
                    {
                        bool result = IuserService.VerifyTFA(Input.verificationCode, Input.Email);
                        if (result)
                        {
                            if (!checkUserr.EnableTwoFactor)
                            {
                                checkUserr.EnableTwoFactor = true;
                            }
                            await CreateAuthenticationCookie(checkUserr, Input.RememberMe);
                        }
                        else
                        {
                            //if (!checkUserr.EnableTwoFactor)
                            //{
                            //    QRbase64string = await IuserService.GoogleAuthenticator(Input.Email);
                            //}
                            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                            return Page();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username/password");
                    return Page();
                }
                return Redirect(returnUrl);
#endif
                var checkUser = context.TblUsers.Where(x => x.Username == Input.Email && x.UserStatus == 1).FirstOrDefault();
                if (checkUser != null)
                {
                    try
                    {
                        bool result = IuserService.VerifyTFA(Input.verificationCode, Input.Email);
                        if (result)
                        {
                            if (!checkUser.EnableTwoFactor)
                            {
                                checkUser.EnableTwoFactor = true;
                            }
                            await CreateAuthenticationCookie(checkUser, Input.RememberMe);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                            return Page();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username/password");
                    return Page();
                }
                return Redirect(returnUrl);
            }
            return Page();
        }
        public async Task CreateAuthenticationCookie(TblUser user, bool RememberMe = false)
        {
            string FullName = user.Firstname + " " + user.Lastname;
            List<Claim> claims = new List<Claim>
            {

                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, FullName),
                new Claim(ClaimTypes.Surname, user.Username),
                new Claim(ClaimTypes.UserData, "Active"),
                new Claim(ClaimTypes.Sid, user.UserId.ToString()),
                new Claim(ClaimTypes.GivenName, user.ShortName),
            };


            var userRoles = (from ur in context.UserRoles
                             join r in context.Roles on ur.RoleId equals r.Id
                             join u in context.TblUsers on ur.UserId equals u.UserId
                             where ur.UserId == user.UserId
                             select r).ToList();

            List<RoleClaim> RoleClaims = new List<RoleClaim>();
            List<UserClaim> UserClaims = new List<UserClaim>();


            //UserClaims

            var userClaimss = (from uclaims in context.UserClaims
                               join u in context.TblUsers on uclaims.UserId equals u.UserId
                               where uclaims.UserId == user.UserId
                               select uclaims).ToList();
            UserClaims.AddRange(userClaimss);
            foreach (UserClaim uc in UserClaims)
            {
                claims.Add(new Claim("permission", uc.Value));
            }

            //roleClaims
            foreach (Role item in userRoles)
            {
                List<RoleClaim> crRole2s = (from rr in context.Roles
                                            join cur in context.RoleClaims on rr.Id equals cur.RoleId
                                            where cur.RoleId == item.Id
                                            select cur).ToList();

                RoleClaims.AddRange(crRole2s);
            }




            foreach (RoleClaim rol in RoleClaims)
            {
                claims.Add(new Claim("permission", rol.Value));
                //claims.Add(new Claim(ClaimTypes.Role, rol.Value));
            }

            foreach (Role rol in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.Name));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            if (RememberMe)
            {
                RememberMe = false;
            }
            else
            {
                RememberMe = true;
            }
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                IsPersistent = RememberMe,
                ExpiresUtc = DateTime.UtcNow.AddHours(3)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "auth");
            ClaimsPrincipal cp = new ClaimsPrincipal(identity);
            //await HttpContext.SignInAsync(cp);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            try
            {
                var Role = userRoles.Select(x => x.Name).FirstOrDefault();
                if (string.IsNullOrEmpty(Role)) Role = "";
                var remoteIpAddress = this.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
                //var FGCLog = new TblFgclog
                //{
                //    Object = "User",
                //    Action = "Login",
                //    Remarks = FullName + " logged in to the system",
                //    PostedBy = FullName + $" ({Role})",
                //    IP = remoteIpAddress?.ToString(),
                //    UserId = user.UserId,
                //    DatePosted = DateTime.Now.DateTime_UK()
                //};
                //await context.TblFgclogs.AddAsync(FGCLog);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
