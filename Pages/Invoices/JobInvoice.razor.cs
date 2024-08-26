using ArdantOffical.Data.ModelVm;
using ArdantOffical.SignalRHub;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ArdantOffical.Data.ModelVm.Users;
using System.Linq;
using ArdantOffical.Data.ModelVm.Dashboard;
using ArdantOffical.Data;
using ArdantOffical.Models;
using ArdantOffical.Services;
using SalesforceSharp;
using Microsoft.AspNetCore.Components.Authorization;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Data.ModelVm.Invoices;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using ArdantOffical.Shared;
using Blazorise.Extensions;
using ICSharpCode.SharpZipLib.Zip;
using ArdantOffical.Components.JobInvoices;
using ArdantOffical.IService;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace ArdantOffical.Pages.Invoices
{
    public partial class JobInvoice
    {
        private int currentPage = 1;
        private int totalPageQuantity;
        private int totalCount = -1;
        public PaginationDTO paginationObj { get; set; } = new PaginationDTO();
        //public List<InvoicesVM> lstInvoices { get; set; }
        //public InvoicesVM Modal = new InvoicesVM();
        public InvoicesVMForTable ListOfRecord { get; set; }
        public List<InvoicesVM> PerPageInvoiceRecords { get; set; }
        public IQueryable<InvoicesVM> PerPageUserRecordIQueryable { get; set; }
        public List<InvoicesVM> AllUserRecords { get; set; }
        /*   Modal Popup Params */
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public TostModel TostModelclass { get; set; } = new();
        public bool IsloaderShow { get; set; } = false;
        public CurrentUserInfoVM Userinfo { get; set; }
        public string ErrorMessage { get; set; }
        public SearchFilterVm searchFilters = new SearchFilterVm();
        private async Task SelectedPage(int page)
        {
            currentPage = page;
            await LoadRecords(page);
        }
        [Inject]
        public AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        public List<SelectListItem> ListOfTablePages = new List<SelectListItem>();
        public List<SelectListItem> lstStatus = new List<SelectListItem>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ListOfTablePages.Add(new SelectListItem() { Text = "50", Value = "50" });
                ListOfTablePages.Add(new SelectListItem() { Text = "75", Value = "75" });
                ListOfTablePages.Add(new SelectListItem() { Text = "100", Value = "100" });
                ListOfTablePages.Add(new SelectListItem() { Text = "200", Value = "200" });
                GetStatus();
                paginationObj = new PaginationDTO();
                paginationObj.QuantityPerPage = 200;

                // await hubConnection.InvokeAsync("SendMessage");
                await LoadRecords();

            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
            }
        }

        public async Task LoadRecords(int page = 1, int quantityPerPage = 200)
        {
            try
            {
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();

                currentPage = page;
                paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
                ListOfRecord = await IinvoiceServices.GetNDISInvoices(paginationObj, Userinfo.SalesforceID);
                if (ListOfRecord != null)
                {
                    PerPageInvoiceRecords = ListOfRecord.Invoices;
                    AllUserRecords = ListOfRecord.Invoices_All;
                    totalPageQuantity = ListOfRecord.TotalPages;
                    totalCount = ListOfRecord.TotalCount;
                }

            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }

        // Download File name
        private string fileName = "Invoices" + DateTime.Now.ToString("ddMMyyyyhhmmss");
        public bool IsSpinnerDownload { get; set; }

        public async Task DownloandExcel()
        {
            IsSpinnerDownload = true;
            await Task.Delay(100);
            byte[] response;
            if (searchFilters.FromDate != null && searchFilters.ToDate != null || (searchFilters.Status != null || searchFilters.SearchKey != null))
            {
                response = IinvoiceServices.CreateReconciliationSheet(PerPageInvoiceRecords);
            }
            else
            {
                response = IinvoiceServices.CreateReconciliationSheet(PerPageInvoiceRecords);
            }
            fileName += ".xlsx";
            // Invoke Js function saveAsFile to save the response as Excel file
            await js.InvokeAsync<object>("saveAsFile", fileName, Convert.ToBase64String(response));
            IsSpinnerDownload = false;
        }

        public bool GetReportSpinner { get; set; }
        public bool IsValidationPopup { get; set; }
        private async Task SearchChanged()
        {

            
            if (searchFilters.FromDate == null && searchFilters.ToDate == null && searchFilters.Status == null && searchFilters.SearchKey == null)
            {
                IsValidationPopup = true;
                return;
            }
            else
            {
                GetReportSpinner = true;

                if (searchFilters.SearchKey != null || searchFilters.Status != null || (searchFilters.FromDate != null && searchFilters.ToDate != null))
                {
                    IsloaderShow = true;
                    await Task.Delay(100);
                    CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();

                    try
                    {
                        currentPage = 1;
                        paginationObj = new PaginationDTO() { Page = paginationObj.Page, QuantityPerPage = paginationObj.QuantityPerPage };
                        ListOfRecord = await IinvoiceServices.GetInvoiceSearch(paginationObj, searchFilters, Userinfo.SalesforceID);
                        if (ListOfRecord != null)
                        {
                            PerPageInvoiceRecords = ListOfRecord.Invoices;
                            AllUserRecords = ListOfRecord.Invoices_All;
                            totalPageQuantity = ListOfRecord.TotalPages;
                            totalCount = ListOfRecord.TotalCount;
                        }
                    }
                    catch (Exception ex)
                    {
                        responseHeader = "ERROR";
                        responseBody = ex.Message;
                        responseDialogVisibility = true;
                        throw;
                    }
                }
                else
                {
                    await LoadRecords(1, paginationObj.QuantityPerPage);
                }
                // await LoadPeople();
                IsloaderShow = false;
                StateHasChanged();
                GetReportSpinner = false;
            }
           
        }
        public void HideValidationPopup()
        {
            IsValidationPopup = false;
        }

        private async Task FormReset()
        {
            IsloaderShow = true;
            await Task.Delay(100);
            PerPageInvoiceRecords = new();
            searchFilters = new();
            await LoadRecords(1, paginationObj.QuantityPerPage);
            StateHasChanged();
            IsloaderShow = false;
        }
        public void GetStatus()
        {
            lstStatus.Add(new SelectListItem() { Value = "Pending", Text = "Pending" });
            lstStatus.Add(new SelectListItem() { Value = "Paid", Text = "Paid" });
            lstStatus.Add(new SelectListItem() { Value = "Overdue", Text = "Overdue" });
            lstStatus.Add(new SelectListItem() { Value = "Sent", Text = "Sent" });
            lstStatus.Add(new SelectListItem() { Value = "Disputed", Text = "Disputed" });

        }
        private void ConnectSalesforce()
        {
            try
            {
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                IsloaderShow = false;
                responseHeader = "ERROR";
                responseBody = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
                responseDialogVisibility = true;

            }
        }
        #region Preview Invoice
        private string htmlContent;
        [Inject]
        public IRazorRendererHelper RazorRendererHelper { get; set; }
        public string InvoiceId { get; set; }
        public bool IsInvoicePreview { get; set; }
        public InvoicesVM InvoiceModal = new InvoicesVM();
        public bool IsSpinner { get; set; }
        public async Task InvoicePreview(string invoiceId)
        {
            IsloaderShow = true;
            await Task.Delay(100);
            InvoiceId = invoiceId;
            InvoiceModal = await IinvoiceServices.GetInvoiceItems(invoiceId);
            GetJobDetails();
            string partialName = "/Views/InvoicePdf.cshtml";
            htmlContent = RazorRendererHelper.RenderPartialToString(partialName, InvoiceModal);
            IsInvoicePreview = true;
            IsloaderShow = false;
        }

        public void HideInvoicePreview()
        {
            IsInvoicePreview = false;
        }

        public void GetJobDetails()
        {
            ConnectSalesforce();
            try
            {
                var record = SFConnect.client.Query<NDIS>("SELECT Id,PrimaryContactEmail__c, Job_Number__c,P_Firstname__c,P_Surname__c,Street_Address__c,P_Phone__c,Suburb__c,Postcode__c FROM NDIS_Job__c  WHERE Status__c= 'Assigned' AND Id='" + InvoiceModal.JobID + "' ");
                InvoiceModal.SentTo = record.FirstOrDefault().PrimaryContactEmail__c;
                InvoiceModal.Job_Number = record.FirstOrDefault().Job_Number__c;
                InvoiceModal.Customer_Name = record.FirstOrDefault().P_Firstname__c + ' ' + record.FirstOrDefault().P_Surname__c;
                InvoiceModal.Address = record.FirstOrDefault().Street_Address__c;
                InvoiceModal.Suburb = record.FirstOrDefault().Suburb__c;
                InvoiceModal.Postcode = record.FirstOrDefault().Postcode__c;
                InvoiceModal.CustomerPhone = record.FirstOrDefault().P_Phone__c;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        #endregion

        #region Add Invoice
        public bool IsAddSpinner { get; set; }
        public async Task CreateNewInvoice()
        {
            IsAddSpinner = true;
            // Subscribe to the navigation completed event
            Navigator.LocationChanged += HandleLocationChanged;

            // Delay for visual effect
            await Task.Delay(500);
            Navigator.NavigateTo("/AddInvoice");
            
        }
        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            // Unsubscribe from the navigation completed event
            Navigator.LocationChanged -= HandleLocationChanged;

            // Hide the spinner after navigation is complete
            IsAddSpinner = false;
            IsloaderShow = false;
        }
        #endregion

        #region Edit Invoice
       
        public async Task EditInvoice(string id)
        {
            IsloaderShow = true;
            // Subscribe to the navigation completed event
            Navigator.LocationChanged += HandleLocationChanged;
            Navigator.NavigateTo($"EditInvoice/{id}");

        }

        #endregion

        #region Delete Invoice
        public bool IsDeleteComfirmation { get; set; }
        public void DeleteInvoice(string id)
        {
            InvoiceModal.Id = id;
            IsDeleteComfirmation = true;
        }
        public async Task DeleteConfirmed(bool value)
        {
            IsloaderShow = true;
          await Task.Delay(20);
            if (value)
            {
               await IinvoiceServices.DeleteInvoice(InvoiceModal.Id);
              await  LoadRecords();
            }
            IsDeleteComfirmation = false;
            IsloaderShow = false;
        }

        #endregion
    }
}
