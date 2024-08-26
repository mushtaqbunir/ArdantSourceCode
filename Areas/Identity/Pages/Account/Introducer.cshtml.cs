using FGC_OnBoarding.Models.IntroducersModels;
using FGC_OnBoarding.Models.Users;
using FGCCore.Data;
using FGCCore.Helpers.Extensions;
using FGCCore.IService;
using FGCCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FGCCore.Areas.Identity.Pages
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class IntroducerModel : PageModel
    {
        private readonly FGCOnboardingContext context;
        private readonly FGCDbContext CMSDbContext;
        private readonly ILogger<IntroducerModel> _logger;
        public ISystemConfiguration SConfig;

        public IntroducerModel(ILogger<IntroducerModel> logger, FGCOnboardingContext context, FGCDbContext cmscontext, ISystemConfiguration _SConfig)
        {
            this.context = context;
            this.SConfig = _SConfig;
            CMSDbContext = cmscontext;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
           
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl = returnUrl ?? Url.Content("~/");
            // Clear the existing external cookie to ensure a clean login process
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //returnUrl = returnUrl == null || returnUrl == "/" ? Url.Content("~/shipments/0") : returnUrl;
            returnUrl = "/TransactionsSummary";
            if (ModelState.IsValid)
            {
                //await _signInManager.SignInAsync(await _userManager.FindByNameAsync(Input.Email), false);
                //return LocalRedirect(returnUrl);
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
#if DEBUG
                //var user1 = await _userManager.FindByNameAsync(Input.Email.ToLower());
                //await _signInManager.SignInAsync(user1, true);
                IntroducerUsers checkUserr = context.IntroducerUsers.Where(x => x.UserName == Input.Email).FirstOrDefault();
                if (checkUserr != null)
                {
                    try
                    {
                        await CreateAuthenticationCookie(checkUserr, Input.RememberMe);
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
                //await userCacheService.SetSessionTime(Input.Email, DateTime.UtcNow);
                return Redirect(returnUrl);
#endif

                var checkUser = context.IntroducerUsers.Where(x => x.UserName == Input.Email && x.Password == Input.Password).FirstOrDefault();
                if (checkUser != null)
                {
                    try
                    {
                        await CreateAuthenticationCookie(checkUser, Input.RememberMe);
                        return Redirect(returnUrl);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    // js.InvokeAsync("OnUserConnect//");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username/password");
                    return Page();
                }


                //if (userTemp.PasswordExpiryInDays > 0 && userTemp.ExpireDate < MyDateTime.Today)
                //{
                //    ModelState.AddModelError(string.Empty, "Your Password has expored as per password policy, please you forgot password option to reset your password");
                //    return Page();
                //}

                //var result = await _signInManager.PasswordSignInAsync(Input.Email.ToLower(), Input.Password, Input.RememberMe, lockoutOnFailure: true);

                //if (result.Succeeded)
                //{
                //    _logger.LogInformation("User logged in.");
                //    //await userCacheService.SetSessionTime(Input.Email, DateTime.UtcNow);
                //    return Redirect(returnUrl);
                //}
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");
                //    return RedirectToPage("./Lockout");
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return Page();
                //}
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        public async Task CreateAuthenticationCookie(IntroducerUsers user, bool RememberMe = false)
        {


            string FullName = user.FirstName + " " + user.LastName;
            List<Claim> claims = new List<Claim>
            {

                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, FullName),
                new Claim(ClaimTypes.Surname, user.UserName),
                new Claim(ClaimTypes.UserData, "Active"),

                new Claim(ClaimTypes.Sid, user.IntroducerId.ToString()),
            };


            var userRoles = (from ur in context.IntroducerUserRole
                             join u in context.IntroducerUsers on ur.UserRoleId equals u.UserRoleId
                             where u.UserId == user.UserId
                             select ur).ToList();

            List<RoleClaim> RoleClaims = new List<RoleClaim>();
            List<UserClaim> UserClaims = new List<UserClaim>();


            //UserClaims

            //var userClaimss = (from uclaims in context.UserClaims
            //                   join u in context.TblUsers on uclaims.UserId equals u.UserId
            //                   where uclaims.UserId == user.UserId
            //                   select uclaims).ToList();
            //UserClaims.AddRange(userClaimss);
            //foreach (UserClaim uc in UserClaims)
            //{
            //    claims.Add(new Claim("permission", uc.Value));
            //}

            //roleClaims
            //foreach (Role item in userRoles)
            //{
            //    List<RoleClaim> crRole2s = (from rr in context.Roles
            //                                join cur in context.RoleClaims on rr.Id equals cur.RoleId
            //                                where cur.RoleId == item.Id
            //                                select cur).ToList();

            //    RoleClaims.AddRange(crRole2s);
            //}




            foreach (RoleClaim rol in RoleClaims)
            {
                //claims.Add(new Claim("permission", rol.Value));
                claims.Add(new Claim(ClaimTypes.Role, rol.Value));
            }

            foreach (IntroducerUserRole rol in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.RoleName));
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
                ExpiresUtc = DateTime.Now.AddMinutes(30)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "auth");
            ClaimsPrincipal cp = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(cp);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


            try
            {
                var Role = userRoles.Select(x => x.RoleName).FirstOrDefault();
                if (string.IsNullOrEmpty(Role)) Role = "";
                var remoteIpAddress = this.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
                var FGCLog = new TblFgclog
                {
                    Object = "Introducer",
                    Action = "Login",
                    Remarks = FullName + " logged in to the system",
                    PostedBy = FullName + $" ({Role})",
                    IP = remoteIpAddress?.ToString(),
                    UserId = user.UserId,
                    DatePosted = DateTime.Now.DateTime_UK()
                };
                await CMSDbContext.TblFgclogs.AddAsync(FGCLog);
                await CMSDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {


            }
           

        }

    }
}
