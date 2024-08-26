using ArdantOffical.Data;
using ArdantOffical.Helpers;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArdantOffical.Areas.Identity.Pages
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {


        private readonly FGCDbContext context;

        private readonly ILogger<LoginModel> _logger;
        public ISystemConfiguration SConfig;
        //private readonly IBusinessProfileServices _businessProfileServices;
        private readonly AuthenticationStateProvider UserauthenticationStateProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginModel(ILogger<LoginModel> logger, FGCDbContext context, ISystemConfiguration _SConfig,
            AuthenticationStateProvider userauthenticationStateProvider,
            IHttpContextAccessor httpContextAccessor
            )
        {

            this.context = context;
            this.SConfig = _SConfig;
            _logger = logger;
            UserauthenticationStateProvider = userauthenticationStateProvider;
            _httpContextAccessor = httpContextAccessor;
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
            //[Required]
            //public string UserKey { get; set; } = string.Empty;

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
            returnUrl = "/";
            if (ModelState.IsValid)
            {

#if DEBUG
                TblUser checkUserr = context.TblUsers.Where(x => x.Username == Input.Email && x.Password == Input.Password).FirstOrDefault();
                //TblUser checkUserr = context.TblUsers.Where(x => x.Username == Input.Email && x.Password == Input.Password && x.UserKey == Input.UserKey).FirstOrDefault();
                if (checkUserr != null)
                {
                    try
                    {
                        ////LoginInputVM obj = new LoginInputVM() { Email = Input.Email, RememberMe= Input.RememberMe };
                        //// return RedirectToPage("./LoginGoogleAuthentication", returnUrl, obj , returnUrl);
                        //var Passparameter = Input.Email + ", " + Input.RememberMe;
                        //return RedirectToPage("./LoginGoogleAuthentication", new { data = Passparameter, ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        await CreateAuthenticationCookie(checkUserr, Input.RememberMe);
                        //await SaveLog();
                        return Redirect(returnUrl);
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

                var checkUser = context.TblUsers.Where(x => x.Username == Input.Email && x.Password == Input.Password && x.UserStatus == 1).FirstOrDefault();
                if (checkUser != null)
                {
                    try
                    {
                        var Passparameter = Input.Email + ", " + Input.RememberMe;
                        await CreateAuthenticationCookie(checkUser, Input.RememberMe);
                        //await SaveLog();
                        return Redirect(returnUrl);
                        //if(checkUser.SkipAuthenticator)
                        //{
                        //    await CreateAuthenticationCookie(checkUser, Input.RememberMe);
                        //    return Redirect(returnUrl);
                        //}
                        //else
                        //{
                        //    return RedirectToPage("./LoginGoogleAuthentication", new { data = Passparameter, ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        //}


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

            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        //public async Task SaveLog()
        //{
        //    var userLog = new UserLog();
        //    userLog.Action = "Login";
        //    userLog.Object = "User";
        //    await _businessProfileServices.AddUsersLog(userLog);
        //}

        //        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //        {
        //            //returnUrl = returnUrl == null || returnUrl == "/" ? Url.Content("~/shipments/0") : returnUrl;
        //            returnUrl = "/";
        //            if (ModelState.IsValid)
        //            {
        //                //await _signInManager.SignInAsync(await _userManager.FindByNameAsync(Input.Email), false);
        //                //return LocalRedirect(returnUrl);
        //                // This doesn't count login failures towards account lockout
        //                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        //#if DEBUG
        //                //var user1 = await _userManager.FindByNameAsync(Input.Email.ToLower());
        //                //await _signInManager.SignInAsync(user1, true);
        //                TblUser checkUserr = context.TblUsers.Where(x => x.Username == Input.Email).FirstOrDefault();
        //                if (checkUserr != null)
        //                {
        //                    try
        //                    {

        //                        await CreateAuthenticationCookie(checkUserr, Input.RememberMe);

        //                    }
        //                    catch (Exception)
        //                    {
        //                        throw;
        //                    }
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, "Invalid username/password");
        //                    return Page();
        //                }
        //                //await userCacheService.SetSessionTime(Input.Email, DateTime.UtcNow);
        //                return Redirect(returnUrl);
        //#endif

        //                var checkUser = context.TblUsers.Where(x => x.Username == Input.Email && x.Password == Input.Password && x.UserKey == Input.UserKey && x.UserStatus == 1).FirstOrDefault();
        //                if (checkUser != null)
        //                {
        //                    try
        //                    {
        //                        await CreateAuthenticationCookie(checkUser, Input.RememberMe);
        //                        return Redirect(returnUrl);
        //                    }
        //                    catch (Exception)
        //                    {
        //                        throw;
        //                    }
        //                    // js.InvokeAsync("OnUserConnect//");
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, "Invalid username/password");
        //                    return Page();
        //                }


        //                //if (userTemp.PasswordExpiryInDays > 0 && userTemp.ExpireDate < MyDateTime.Today)
        //                //{
        //                //    ModelState.AddModelError(string.Empty, "Your Password has expored as per password policy, please you forgot password option to reset your password");
        //                //    return Page();
        //                //}

        //                //var result = await _signInManager.PasswordSignInAsync(Input.Email.ToLower(), Input.Password, Input.RememberMe, lockoutOnFailure: true);

        //                //if (result.Succeeded)
        //                //{
        //                //    _logger.LogInformation("User logged in.");
        //                //    //await userCacheService.SetSessionTime(Input.Email, DateTime.UtcNow);
        //                //    return Redirect(returnUrl);
        //                //}
        //                //if (result.RequiresTwoFactor)
        //                //{
        //                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        //                //}
        //                //if (result.IsLockedOut)
        //                //{
        //                //    _logger.LogWarning("User account locked out.");
        //                //    return RedirectToPage("./Lockout");
        //                //}
        //                //else
        //                //{
        //                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //                //    return Page();
        //                //}
        //            }

        //            // If we got this far, something failed, redisplay form
        //            return Page();
        //        }


        public async Task CreateAuthenticationCookie(TblUser user, bool RememberMe = false)
        {

            string FullName = user.Firstname + " " + user.Lastname;
            List<Claim> claims = new List<Claim>
            {

                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, FullName),
                new Claim(ClaimTypes.Surname, user.Username),
                new Claim(ClaimTypes.UserData, "Active"),
                new Claim(ClaimTypes.Sid, user.UserId.ToString())               
               
                //new Claim(ClaimTypes.GivenName, user.ShortName),
            };
            if(user.SalesforceID!=null)
            {
                claims.Add(new Claim(ClaimTypes.PrimarySid, user.SalesforceID.ToString()));
            }
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
               // claims.Add(new Claim(ClaimTypes.Role, rol.Value));
            }
            foreach (Role rol in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.Name));
            }
            //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            if (RememberMe)
            {
                RememberMe = false;
            }
            else
            {
                RememberMe = true;
            }
            //AuthenticationProperties authProperties = new AuthenticationProperties
            //{
            //    //IsPersistent = RememberMe,
            //    //IsPersistent = false,
            //    //ExpiresUtc = DateTime.UtcNow.AddHours(12),
            //};
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddHours(3),
                IsPersistent = RememberMe,
            };
            //ClaimsIdentity identity = new ClaimsIdentity(claims, "auth");
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal cp = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,cp, authProperties);
            //await HttpContext.SignInAsync(cp, authProperties);
            //await HttpContext.SignInAsync(cp);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cp, authProperties);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            //var log = "";
            //foreach (var item in authProperties.Items)
            //{
            //    log = log + "," + item.Key+"= "+item.Value;
            //}
            //var logList = new List<TblFgclog>();
            //var FGCLog1 = new TblFgclog
            //{
            //    Object = "Login ",
            //    Action = "Expire Time Log",
            //    Remarks = $"{log}",
            //    UserId = user.UserId,
            //    DatePosted = DateTime.Now.DateTime_UK()
            //};
            //logList.Add(FGCLog1);
            try
            {
                var Role = userRoles.Select(x => x.Name).FirstOrDefault();
                if (string.IsNullOrEmpty(Role)) Role = "";
                var remoteIpAddress = this.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
               
                await context.SaveChangesAsync();
                //set in global varaible
                UserHelper.FullName = FullName;
                UserHelper.Role = Role;
                UserHelper.UserEmail = user.Email;
                UserHelper.UserId = user.UserId;
                UserHelper.UserName = user.Username;
                UserHelper.SalesforceID=user.SalesforceID;
            }
            catch (Exception)
            {


            }


            //AuthenticationManager.(new AuthenticationProperties
            //{
            //    IsPersistent = RememberMe
            //}, claims);
            //    await HttpContext.SignInAsync(
            //CookieAuthenticationDefaults.AuthenticationScheme,
            //new ClaimsPrincipal(claimsIdentity),
            //authProperties);


            //      var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(identity));

        }

      
    }

    public class LoginModel2 : PageModel
    {
        public string GetURL()
        {
            var DomaiName = HttpContext.Request.Host;
            FGCExtensions.DomaiName = DomaiName.ToString();
            return DomaiName.ToString();
        }

    }
    }
