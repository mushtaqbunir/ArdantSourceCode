using ArdantOffical.Data;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers.Extensions
{
    public static class CurrentUserExtensionscs
    {
        public static async Task<CurrentUserInfoVM> CurrentUser(this AuthenticationStateProvider AuthenticationStateProvider)
        {
            CurrentUserInfoVM CurrentuserInfo = new CurrentUserInfoVM();
            AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            ClaimsPrincipal user = authState.User;
            //var namee = Currentuserinfo.Identity.Name;
            var claimsIdentity = (ClaimsIdentity)user.Identity;
            var UserId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Sid);//get user id
            var UserEmail = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email);//get user email
            var UserName = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Surname);//get user name
            var OnlineStatus = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.UserData);//get Online Status
            var Role = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Role);//get Role
            var ShortName = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.GivenName);//get short name
            var SalesforceID = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid); // get the saleforce ID
            var otProfilePic = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Thumbprint); // get the OT Profile Picture
            var folder = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Locality); // get the OT Profile Picture folder
            if (UserId != null)
            {
                CurrentuserInfo.UserId = Convert.ToInt32(UserId.Value);
                CurrentuserInfo.UserEmail = UserEmail.Value;
                CurrentuserInfo.UserName = UserName.Value;
                CurrentuserInfo.FullName = user.Identity.Name;
                CurrentuserInfo.OnlineStatus = OnlineStatus?.Value;
                CurrentuserInfo.Role = Role?.Value;
                CurrentuserInfo.ShortName = ShortName?.Value;
                CurrentuserInfo.SalesforceID = SalesforceID?.Value;

                //var CurrentUser = await _userManager.GetUserAsync(user);
            }
            return CurrentuserInfo;

        }
    }
}
