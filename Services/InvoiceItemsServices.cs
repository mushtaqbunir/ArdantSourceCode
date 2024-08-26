using ArdantOffical.Components.JobInvoices;
using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm.Invoices;
using ArdantOffical.Helpers;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Models;
using Blazorise;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesforceSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Services
{
    public class InvoiceItemsServices : ControllerBase, IInvoiceItemsServices
    {
        private readonly FGCDbContext context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public InvoiceItemsServices(FGCDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            this.context = context;
            _authenticationStateProvider = authenticationStateProvider;

        }

        public Microsoft.AspNetCore.Http.HttpContext GetHttpContext()
        {
            return HttpContext;
        }
        [HttpPost]
        public async Task<Exception> AddInvoiceItem(List<InvoiceItemsVm> Items)
        {
            try
            {
                var existingItems = SFConnect.client.Query<InvoiceItems>($"SELECT Id,Item__c FROM NDIS_Invoice_Items__c WHERE NDIS_Invoice__c = '{Items.FirstOrDefault()?.InvoiceId}'");
                if (existingItems.Count > 0)
                {
                    foreach (var existingItem in existingItems)
                    {
                        SFConnect.client.Delete("NDIS_Invoice_Items__c", existingItem.Id);
                    }
                }

                foreach (var item in Items)
                {
                    if (item.Amount > 0)
                    {
                        string result = SFConnect.client.Create("NDIS_Invoice_Items__c",
                   new
                   {
                       Item__c = item.ItemName,
                       Date__c = item.Date?.ToString("yyyy-MM-ddTHH:mm:ssK"),
                       Amount__c = item.TotalAmount,
                       Description__c = item.Description,
                       NDIS_Invoice__c = item.InvoiceId,
                       Qty_Hours__c = item.Hours,
                       Qty_KMs__c = item.KM,
                       Qty_Minutes__c = item.Minutes,
                       //Quantity_Time__c = Model.QuantityTime,
                       Tax_Rate__c = item.Tax,
                       Unit__c = item.Unit,
                       Rate__c = item.Rate,
                       Unit_Price__c = item.UnitPrice,
                   });
                    }
                   
                }

                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }

        public async Task<Exception> DeleteInvoiceItem(string Id)
        {
            try
            {
                SFConnect.client.Delete("NDIS_Invoice_Items__c", Id);
                return new Ok("1");
            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }

        public async Task<InvoiceItemsVm> GetInvoiceByID(string Id)
        {
            try
            {
                var records = SFConnect.client.Query<InvoiceItems>($"SELECT Id, Date__c,Item__c,Description__c,Amount__c," +
                    $"Qty_Hours__c,Qty_KMs__c,Qty_Minutes__c,Rate__c,Tax_Rate__c,Unit__c,Unit_Price__c FROM NDIS_Invoice_Items__c  WHERE  Id = '{Id}'");
                var queryable = (from c in records
                                 select new InvoiceItemsVm
                                 {
                                     Id = c.Id,
                                     ItemName = c.Item__c,
                                     Description = c.Description__c,
                                     //Amount = c.Amount__c,
                                     Hours = c.Qty_Hours__c,
                                     KM = c.Qty_KMs__c,
                                     Minutes = c.Qty_Minutes__c,
                                     Rate = c.Rate__c,
                                     Tax = c.Tax_Rate__c.ToString(),
                                     Unit = c.Unit__c,
                                     UnitPrice = c.Unit_Price__c,

                                 }).FirstOrDefault();

                return queryable;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<InvoiceItemsVm> GetInvoiceItem(string invoiceId)
        {
            InvoiceItemsVm lstInvoiceItems = new InvoiceItemsVm();
            try
            {
                var records = SFConnect.client.Query<InvoiceItems>($"SELECT Id, Date__c,Item__c,Description__c,Amount__c," +
                    $"Qty_Hours__c,Qty_KMs__c,Qty_Minutes__c,Rate__c,Tax_Rate__c,Unit__c,Unit_Price__c FROM NDIS_Invoice_Items__c  WHERE  NDIS_Invoice__c = '{invoiceId}'");
                var queryable = (from c in records
                                 select new InvoiceItemsVm
                                 {
                                     Id = c.Id,
                                     ItemName = c.Item__c,
                                     Description = c.Description__c,
                                     //Amount = c.Amount__c,
                                     Hours = c.Qty_Hours__c,
                                     KM = c.Qty_KMs__c,
                                     Minutes = c.Qty_Minutes__c,
                                     Rate = c.Rate__c,
                                     Tax = c.Tax_Rate__c.ToString(),
                                     Unit = c.Unit__c,
                                     UnitPrice = c.Unit_Price__c,

                                 });

                lstInvoiceItems.InvoiceItemList = queryable.ToList();

                return lstInvoiceItems;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }


        // using this query creating pdf for NDIS Jobs

        [HttpGet]
        public async Task<InvoiceItemsVm> GetInvoiceItems(string invoiceId)
        {
            InvoiceItemsVm lstInvoiceItems = new InvoiceItemsVm();
            try
            {

                var jobRecords = SFConnect.client.Query<NDIS>($@"SELECT Id, P_Firstname__c,P_Surname__c,Street_Address__c,Suburb__c, Postcode__c FROM 
        NDIS_Job__c WHERE  
        Id IN (SELECT NDIS_Job__c FROM NDIS_Invoice__c WHERE Id = '{invoiceId}')");


                var invoiceRecords = SFConnect.client.Query<Invoice>($@"SELECT Id, Invoice_Date__c, Status__c FROM NDIS_Invoice__c WHERE Id = '{invoiceId}'");

                var records = SFConnect.client.Query<InvoiceItems>($"SELECT Id, Date__c,Item__c,Description__c,Amount__c," +
                     $"Qty_Hours__c,Qty_KMs__c,Qty_Minutes__c,Rate__c,Tax_Rate__c,Unit__c,Unit_Price__c FROM NDIS_Invoice_Items__c  WHERE  NDIS_Invoice__c = '{invoiceId}'");
                var jobRecord = jobRecords.FirstOrDefault();
                var invoiceRecord = invoiceRecords.FirstOrDefault();
                lstInvoiceItems.SubTotal = records.Select(r => r.Amount__c).Sum();
                lstInvoiceItems.NetAmount = records.Sum(r => r.Amount__c * r.Tax_Rate__c / 100);
                lstInvoiceItems.NetAmount = lstInvoiceItems.SubTotal - lstInvoiceItems.NetAmount;
                lstInvoiceItems.InvoiceId = invoiceId;
                var queryable = records.Select(ii => new InvoiceItemsVm
                {
                    CustomerName = jobRecord.P_Firstname__c + " " + jobRecord.P_Surname__c,
                    CustomerAddress = jobRecord.Street_Address__c,
                    CustomerSubUrb = jobRecord.Suburb__c,
                    CustomerPostCode = jobRecord.Postcode__c,
                    Id = ii.Id,
                    ItemName = ii.Item__c,
                    Description = ii.Description__c,
                    //Amount = ii.Amount__c,
                    Hours = ii.Qty_Hours__c,
                    KM = ii.Qty_KMs__c,
                    Minutes = ii.Qty_Minutes__c,
                    Rate = ii.Rate__c,
                    Tax = ii.Tax_Rate__c.ToString(),
                    Unit = ii.Unit__c,
                    UnitPrice = ii.Unit_Price__c,
                    InvoiceDate = invoiceRecord.Invoice_Date__c,
                    InvoiceStatus = invoiceRecord.Status__c
                });

                lstInvoiceItems.InvoiceItemList = queryable.ToList();

                
                return lstInvoiceItems;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<Exception> UpdateInvoiceItem(InvoiceItemsVm Model)
        {
            try
            {
                SFConnect.client.Update("NDIS_Invoice_Items__c", Model.Id, new
                {
                    Item__c = Model.ItemName,
                    Date__c = Model.Date?.ToString("yyyy-MM-ddTHH:mm:ssK"),
                    Amount__c = Model.Amount,
                    Description__c = Model.Description,
                    NDIS_Invoice__c = Model.InvoiceId,
                    Qty_Hours__c = Model.Hours,
                    Qty_KMs__c = Model.KM,
                    Qty_Minutes__c = Model.Minutes,
                    // Quantity_Time__c = Model.QuantityTime,
                    Rate__c = Model.Rate,
                    Tax_Rate__c = Model.Tax,
                    Unit__c = Model.Unit,
                    Unit_Price__c = Model.UnitPrice,
                });
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }


        // Get all assigned jobs for a specific user, sfId is SalesForce id
        [HttpGet]
        public async Task<List<SelectListItem>> GetAssignedJobs()
        {

            try
            {
                CurrentUserInfoVM Userinfo = await _authenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<NDIS>($"SELECT Id,Job_Number__c, P_Firstname__c,P_Surname__c  FROM NDIS_Job__c  WHERE Status__c ='Assigned' AND  OT__c = '{Userinfo.SalesforceID}'");
                var selectListItems = records.Select(r => new SelectListItem
                {
                    Text = r.P_Surname__c + "," + r.P_Firstname__c + " (" + r.Job_Number__c + ")",
                    Value = r.Id
                }).ToList();

                return selectListItems;


            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        // Get all assigned jobs for a specific user, sfId is SalesForce id
        [HttpGet]
        public async Task<List<NDIS>> GetAssignedNDISJobs()
        {

            try
            {
                CurrentUserInfoVM Userinfo = await _authenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<NDIS>($"SELECT Id,Job_Number__c,P_Firstname__c,P_Surname__c FROM NDIS_Job__c  WHERE Status__c ='Assigned' AND  OT__c = '{Userinfo.SalesforceID}'");
                var selectListItems = records.Select(r => new NDIS
                {
                    Job_Number__c =r.P_Surname__c + "," + r.P_Firstname__c + " (" + r.Job_Number__c + ")",
                    Id = r.Id,
                    P_Firstname__c = r.P_Firstname__c,
                    P_Surname__c = r.P_Surname__c
                }).ToList();

                return selectListItems;


            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

       

        public Task<Exception> AddInvoiceItem(InvoiceItemsVm Model)
        {
            throw new NotImplementedException();
        }
    }
}
