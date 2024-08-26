using ArdantOffical.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class AuthorizationCheck
    {
        [Inject]
        protected NavigationManager Navigation { get; set; }
        [Inject]
        protected IServiceScopeFactory ScopeFactory { get; set; }
        [Inject]
        protected AuthenticationStateProvider authenticationStateAsync { get; set; }
        [Parameter]
        public RouteData route { get; set; }

        protected override Task OnParametersSetAsync()
        {
            var authstate = authenticationStateAsync.GetAuthenticationStateAsync();
            var User = authstate.Result.User;
            if (User.Identity.IsAuthenticated)
            {
                if (!CheckAuthorization(User)) //check role and its authorization here
                {
                    Navigation.NavigateTo("identity/account/Login", true);
                }
            }
            
            return base.OnParametersSetAsync();
        }

        public bool CheckAuthorization(ClaimsPrincipal User)
        {
            try
            {
                if (route.PageType.Name == "Index") return true;

                var context = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<FGCDbContext>();
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var UserId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Sid).Value);

                return (from  userClaims in context.UserClaims
                             join menuLink in context.MenuItems on userClaims.Value equals menuLink.MenuName
                             where userClaims.UserId == UserId && menuLink.ActionLink!="/" && Navigation.Uri.ToLower().Contains(menuLink.ActionLink.ToLower())
                             select menuLink.ActionLink)
                             .Any();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
