using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.SystemLog;
using ArdantOffical.Helpers;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ArdantOffical.Services
{
    public class SystemLogServices : ControllerBase, ILogServices
    {
        private readonly FGCDbContext context;
        private readonly AuthenticationStateProvider UserauthenticationStateProvider;
        //public IHttpContextAccessor _httpContext { get; private set; }
        public SystemLogServices(FGCDbContext context, AuthenticationStateProvider _UserauthenticationStateProvider)
        {
            this.context = context;
            //this.Environment = _Environment;
            //this.EEnvironment = _EEnvironment;
            //var gg = myConfiguration.Value.token;
            this.UserauthenticationStateProvider = _UserauthenticationStateProvider;
            //this._httpContext = contextAccessor;
        }

        //public Task<List<SystemLogVM>> SystemLogDownloadExcel(SearchParams searchParams)
        //{
        //    SystemLogPagination lstSystemLog = new SystemLogPagination();
        //    //Expression<Func<TblFgclog, bool>> SearchParams = searchParams.UserID.ToString() == "1111"
        //    //    ? (t => t.DatePosted >= searchParams.StartDate && t.DatePosted <= searchParams.EndDate)
        //    //    : (t => t.UserId == searchParams.UserID && t.DatePosted >= searchParams.StartDate && t.DatePosted <= searchParams.EndDate);
        //    //IQueryable<SystemLogVM> queryable = from c in context.TblFgclogs.Where(SearchParams)
        //    //                                    orderby c.Id descending
        //    //                                    select new SystemLogVM
        //    //                                    {
        //    //                                        ID = c.Id,
        //    //                                        Object = c.Object,
        //    //                                        Action = c.Action,
        //    //                                        Status = c.Status,
        //    //                                        OldValue = c.OldValue,
        //    //                                        NewValue = c.NewValue,
        //    //                                        Remarks = c.Remarks,
        //    //                                        PostedBy = c.PostedBy,
        //    //                                        UserID = c.UserId,
        //    //                                        CRID = c.Crid,
        //    //                                        DatePosted = c.DatePosted,
        //    //                                        CommentID = c.CommentId,
        //    //                                        HdID = c.HdId,


        //    //                                    };

        //    //lstSystemLog.SystemLogAll_Export = queryable.ToList();

        //    return Task.FromResult(lstSystemLog.SystemLogAll_Export);
        //}

        public void SaveSystemLog(SystemLogVM log)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    //  MethodB();
                    CurrentUserInfoVM Userinfo = UserauthenticationStateProvider.CurrentUser().ConfigureAwait(false).GetAwaiter().GetResult();
                    // context.Entry(record).State = EntityState.Added;
                    context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.RollbackAsync();
                }
            }
        }


    }
}
