using ArdantOffical.Data;
using ArdantOffical.Helpers;
using DocumentFormat.OpenXml.InkML;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.IO;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Models;
using ArdantOffical.IService;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArdantOffical.Services
{
    public class AttachmentServices : ControllerBase, IAttachmentServices
    {
        private readonly HttpClient httpClient;
        IServiceScopeFactory _serviceScope;
        private string rootFolder;
        public IWebHostEnvironment Environment;
        private readonly FGCDbContext context;
        private readonly AuthenticationStateProvider UserauthenticationStateProvider;
        public IAuthorizationHandler IAuth;
        private readonly IServiceCollection _services;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private EmailService _emailSender;
        public AttachmentServices(FGCDbContext context,
            AuthenticationStateProvider _UserauthenticationStateProvider,
            HttpClient httpClient,
            IServiceScopeFactory serviceScope,
            IAuthorizationHandler _IAuth,
            IWebHostEnvironment Environment,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this.context = context;
            this.Environment = Environment;
            //var gg = myConfiguration.Value.token;
            this.UserauthenticationStateProvider = _UserauthenticationStateProvider;
            rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/Documents");
            _serviceScope = serviceScope;
            this.IAuth = _IAuth;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = new EmailService();
        }
        public async Task<Exception> SaveAttachments(AttachmentsVm attachmentsVm)
        {
            CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();

            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    //MaintenanceStatus = MaintenanceStatus.NotInitiated,

                    Attachments Addattachment = new Attachments();
                    Addattachment.Title = attachmentsVm.Title;
                    Addattachment.Path = attachmentsVm.Path;
                    Addattachment.SalesforceID = attachmentsVm.SalesforceID;
                    Addattachment.PostedBy = Userinfo.FullName;
                    Addattachment.DatePosted = DateTime.Now;
                    Addattachment.Folder = attachmentsVm.Folder;
                    Addattachment.SalesforceID = attachmentsVm.SalesforceID;
                    Addattachment.Title = attachmentsVm.Title;
                    Addattachment.Type = "Jobs";


                    context.Attachments.Add(Addattachment);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public async Task<List<AttachmentsVm>> GetAllAttachments(string jobId)
        {
            try
            {
                return await (from a in context.Attachments
                              where a.SalesforceID == jobId && a.IsDeleted == false
                              select new AttachmentsVm()
                              {
                                  ID = a.ID,
                                  Title = a.Title,
                                  Path = a.Path,
                                  Folder = a.Folder,
                                  PostedBy = a.PostedBy,
                                  DatePosted=a.DatePosted
                              }).ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task DeleteAttachments(int fileId)
        {
            try
            {

                var attachment = await context.Attachments.FindAsync(fileId);
                attachment.IsDeleted = true;
                context.Attachments.Update(attachment);
                context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
