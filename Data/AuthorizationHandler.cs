using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ArdantOffical.Data.AuthorizationHandler;

namespace ArdantOffical.Data
{
    public interface IAuthorizationHandler
    {
        Task ConfigureServices(IServiceCollection services);
    }
    public class AuthorizationHandler : IAuthorizationHandler
    {
        public IUsersServices _iusersServices;

        public AuthorizationHandler(IUsersServices iusersServices)
        {
            this._iusersServices = iusersServices;

        }

        public List<MenuItem> ListOfMenuItem = new();
        public async Task ConfigureServices(IServiceCollection services)
        {

            //var contexttt = services.BuildServiceProvider().GetService<IUsersServices>();
            IQueryable<MenuItem> ListOfMenuItem = _iusersServices.GetMenuItemIQueryable();
            services.AddAuthorization(auth =>
            {
                foreach (var item in ListOfMenuItem)
                {
                    auth.AddPolicy(item.MenuName, policy => policy.RequireClaim("permission", item.MenuName));

                }
            });

            foreach (var item in ListOfMenuItem)
            {
                _iusersServices.addnew(item.MenuItemID);
            }
        }
        public interface IServiceCollectionProvider
        {
            IServiceCollection ServiceCollection { get; }
        }
        public sealed class ServiceCollectionProvider : IServiceCollectionProvider
        {
            public ServiceCollectionProvider(IServiceCollection serviceCollection)
            {
                ServiceCollection = serviceCollection;
            }
            public IServiceCollection ServiceCollection { get; }
        }
    }
    public interface ICreateIAuthorizationPolicy
    {
        Task CreatePolicy();
    }
    public class CreateIAuthorizationPolicy : ICreateIAuthorizationPolicy
    {
        public IAuthorizationHandler IAuth;
        private readonly IServiceCollection _services;
        public CreateIAuthorizationPolicy(IAuthorizationHandler _IAuth, IServiceCollectionProvider serviceCollectionProvider)
        {
            this.IAuth = _IAuth;
            this._services = serviceCollectionProvider.ServiceCollection;
        }

        public async Task CreatePolicy()
        {
            await IAuth.ConfigureServices(_services);//Create Policy 
        }
    }

}
