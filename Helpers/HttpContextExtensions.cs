using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers
{
    public static class HttpContextExtensions
    {
        public static async Task<int> InsertPaginationParameterInResponse<T>(this HttpContext httpContext, IQueryable<T> queryable, int recordsPerPage)
        {
            double count = queryable.Count();
            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
            // httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }
        public static int InsertPaginationParameterInResponseDate<T>(this HttpContext httpContext, IQueryable<T> queryable, int recordsPerPage)
        {
            double count = queryable.Count();
            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
            // httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }

        public static async Task<int> InsertPaginationParameterInResponseList<T>(this HttpContext httpContext, List<T> queryable, int recordsPerPage)
        {
            double count = queryable.Count();
            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
            // httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }

        public static int InsertPaginationParameterInResponseList<T>(List<T> queryable, int recordsPerPage)
        {
            double count = queryable.Count();
            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
        }


        public static int InsertPaginationParameterInResponseDateView<T>(this IQueryable<T> queryable, int recordsPerPage)
        {
            double count = queryable.Count();
            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
            // httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }

        public static int InsertPaginationParameterInResponseNew<T>(this IQueryable<T> queryable, int recordsPerPage)
        {
            double count = queryable.Count();
            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
            // httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }
        public static async Task<int> InsertPaginationParameterInResponsePages(this HttpContext httpContext, double count, int recordsPerPage)
        {

            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
            // httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }
        public static async Task<int> InsertPaginationParameterInResponse22(this double count, int recordsPerPage)
        {

            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
            // httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }
        public static async Task<int> InsertPaginationParameterInResponseForComponents<T>(this IQueryable<T> queryable, int recordsPerPage)
        {
            double count = queryable.Count();
            double pagesQuantity = Math.Ceiling(count / recordsPerPage);
            return Convert.ToInt32(pagesQuantity);
            // httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }
        public static void ConfigureServicess(this IServiceCollection services)
        {
            var contexttt = services.BuildServiceProvider().GetService<IUsersServices>();
            List<MenuItem> ListOfMenuItem = contexttt.GetMenuItemList().ConfigureAwait(false).GetAwaiter().GetResult();
            ListOfMenuItem = ListOfMenuItem.Where(x => x.MenuItemParentID != null).ToList();
            services.AddAuthorization(auth =>
            {
                foreach (var item in ListOfMenuItem)
                {
                    auth.AddPolicy(item.MenuName, policy => policy.RequireClaim("permission", item.MenuName));

                }
            });

        }
        public static async Task ConfigureServicess2(this IServiceCollection services)
        {
            var contexttt = services.BuildServiceProvider().GetService<IUsersServices>();
            List<MenuItem> ListOfMenuItem = await contexttt.GetMenuItemList();
            ListOfMenuItem = ListOfMenuItem.Where(x => x.MenuItemParentID != null).ToList();
            services.AddAuthorization(auth =>
            {
                foreach (var item in ListOfMenuItem)
                {
                    auth.AddPolicy(item.MenuName, policy => policy.RequireClaim("permission", item.MenuName));

                }
            });

        }

    }
}
