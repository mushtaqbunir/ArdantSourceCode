using ArdantOffical.Data.ModelVm.Invoices;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ArdantOffical.Models;
using ArdantOffical.Services;
using System;
using SalesforceSharp;
using Radzen.Blazor;
using Radzen;
using System.Text.RegularExpressions;
using ArdantOffical.Components.JobInvoices;
using ArdantOffical.Helpers.Enums;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using Microsoft.AspNetCore.Components.Authorization;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ArdantOffical.Components.ClinicalData;
using Microsoft.JSInterop;
using System.Threading;
using System.Runtime.CompilerServices;

namespace ArdantOffical.Pages.Invoices
{
    public partial class AddInvoice
    {
        [Parameter]
        public string JobID { get; set; }
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
        [Inject]
        AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        public CurrentUserInfoVM Userinfo { get; set; }
        public string ErrorMessage { get; set; }
        public InvoicesVM InvoiceModal = new InvoicesVM();
        public List<InvoiceItemsVm> Items_Copy = new List<InvoiceItemsVm>();
        public InvoiceItemsVm Modal = new InvoiceItemsVm();
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }

        public string rootFolder;

        public bool IsSpinner { get; set; }


        public List<SelectListItem> lstStatus = new List<SelectListItem>();
        public List<SelectListItem> lstAssignedJobs = new List<SelectListItem>();
        //public List<NDIS> lstJobs = new List<NDIS>();
        public List<SelectListItem> lstJobs = new List<SelectListItem>();
        public List<SelectListItem> ItemsList = new();
        public List<SelectListItem> TaxesList = new();
        public List<SelectListItem> RatesList = new();
        public List<SelectListItem> lstUoM = new();

        [Inject]
        public FGCDbContext Context { get; set; }
        [Inject]
        IWebHostEnvironment Environment { get; set; }
        private string baseUrl;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                try
                {
                    ConnectSalesforce();
                    await DefaultRows();
                    GetItems();
                    GetUoM();
                    GetStatus();
                    GetTaxList();
                    GetRateList();

                    baseUrl = Navigator.BaseUri;
                    rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/Documents/");
                    Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    InvoiceModal.OT = Userinfo.SalesforceID;
                    await LoadAssignedJobs();

                }
                catch (SalesforceException ex)
                {
                    IsloaderShow = false;
                    responseHeader = "ERROR";
                    responseBody = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
                    responseDialogVisibility = true;

                }
            }
        }
       
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(JobID))
                {
                    InvoiceModal.JobID = JobID;
                    Modal.Rate = "193.99";
                    Modal.Tax = "0.0";
                    GetReceipiantEmail();
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

        public async Task LoadAssignedJobs()
        {

            //lstJobs = await IinvoiceServicesItems.GetAssignedNDISJobs();
            try
            {
                lstJobs = await IinvoiceServicesItems.GetAssignedJobs();
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }

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

        public Task SetJobID(string jobId)
        {
            try
            {
                InvoiceModal.JobID = jobId;
                GetReceipiantEmail();

            }
            catch (Exception ex)
            {
                TostModelclass = ex.Message.AlertErrorMessage();
            }

            return Task.CompletedTask;
        }
        public void GetReceipiantEmail()
        {
            ConnectSalesforce();
            try
            {
                var record = SFConnect.client.Query<NDIS>("SELECT Id,PrimaryContactEmail__c, Job_Number__c,P_Firstname__c,P_Surname__c,Street_Address__c,Suburb__c,Postcode__c, NDIS_Plan_Number__c  FROM NDIS_Job__c  WHERE Status__c= 'Assigned' AND Id='" + InvoiceModal.JobID + "' ");
                if (record.Count > 0)
                {
                    InvoiceModal.SentTo = record.FirstOrDefault().PrimaryContactEmail__c;
                    InvoiceModal.Job_Number = record.FirstOrDefault().Job_Number__c;
                    InvoiceModal.Customer_Name = record.FirstOrDefault().P_Firstname__c + ' ' + record.FirstOrDefault().P_Surname__c;
                    InvoiceModal.Address = record.FirstOrDefault().Street_Address__c;
                    InvoiceModal.Suburb = record.FirstOrDefault().Suburb__c;
                    InvoiceModal.Postcode = record.FirstOrDefault().Postcode__c;
                    InvoiceModal.NDISNumber = record.FirstOrDefault().NDIS_Plan_Number__c;
                }
             
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }


        }

        public void OnChangeState(ChangeEventArgs e)
        {
            InvoiceModal.Status = e.Value.ToString();
        }

        public void OnJobSelected(ChangeEventArgs e)
        {
            InvoiceModal.JobID = e.Value.ToString();
            GetReceipiantEmail();
        }

        public void GetStatus()
        {
            lstStatus.Add(new SelectListItem() { Value = "Pending", Text = "Pending" });
            lstStatus.Add(new SelectListItem() { Value = "Paid", Text = "Paid" });
            lstStatus.Add(new SelectListItem() { Value = "Overdue", Text = "Overdue" });
            lstStatus.Add(new SelectListItem() { Value = "Sent", Text = "Sent" });
            lstStatus.Add(new SelectListItem() { Value = "Disputed", Text = "Disputed" });

        }
        public void GetTaxList()
        {
            try
            {
                TaxesList = IinvoiceServices.GetTaxesList();
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }
        }

        public void GetRateList()
        {
            try
            {
                RatesList = IinvoiceServices.GetRateList();
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }
        }

        void OnChange(object value)
        {
            try
            {
                if (value != null)
                {
                    var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;
                    Console.WriteLine($"Value changed to {str}");
                   
                }
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }

        }

        void OnTaxChange(object value)
        {
            try
            {
               // CalculateSubTotal();
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }

        }

        private void CalculateSubTotal()
        {
            SubTotal = InvoiceModal.InvoiceItemList.Sum(x => x.Amount).Value;
            TaxAmount = InvoiceModal.InvoiceItemList.Sum(x => x.TaxAmount);
            Total = InvoiceModal.InvoiceItemList.Sum(x => x.TotalAmount);
        }

        public bool IsNDISTravel { get; set; } = false;
        private void GetItems()
        {
            ItemsList.Add(new SelectListItem() { Value = "NDIS Travel", Text = "NDIS Travel" });
            ItemsList.Add(new SelectListItem() { Value = "NDIS Report", Text = "NDIS Report" });
            ItemsList.Add(new SelectListItem() { Value = "NDIS Communication", Text = "NDIS Communication" });
            ItemsList.Add(new SelectListItem() { Value = "NDIS Assessment of Participant", Text = "NDIS Assessment of Participant" });
            ItemsList.Add(new SelectListItem() { Value = "NDIS Meeting", Text = "NDIS Meeting" });
            ItemsList.Add(new SelectListItem() { Value = "NDIS Site Assessment", Text = "NDIS Site Assessment" });
            ItemsList.Add(new SelectListItem() { Value = "NDIS Equipment Trial", Text = "NDIS Equipment Trial" });
            ItemsList.Add(new SelectListItem() { Value = "NDIS Consumables", Text = "NDIS Consumables" });
            ItemsList.Add(new SelectListItem() { Value = "NDIS Intervention/ Therapy", Text = "NDIS Intervention/ Therapy" });

        }
        private void GetUoM()
        {
            lstUoM.Add(new SelectListItem() { Value = "Minutes", Text = "Minutes" });
            lstUoM.Add(new SelectListItem() { Value = "KM", Text = "KM" });

        }

        public string Id { get; set; }
        public string ActionName { get; set; } = "Save";
        public async Task EditInvoice(string id)
        {
            Id = id;
            Modal = await IinvoiceServicesItems.GetInvoiceByID(Id);
            ActionName = "Update";
        }

        #region Radzden Data Grid

        RadzenDataGrid<InvoiceItemsVm> invoiceItemsGrid;
        DataGridEditMode editMode = DataGridEditMode.Multiple;

        List<InvoiceItemsVm> invoiceItemToInsert = new List<InvoiceItemsVm>();
        List<InvoiceItemsVm> invoiceItemToUpdate = new List<InvoiceItemsVm>();

        void Reset()
        {
            invoiceItemToInsert.Clear();
            invoiceItemToUpdate.Clear();
        }

        void Reset(InvoiceItemsVm invoiceItem)
        {
            invoiceItemToInsert.Remove(invoiceItem);
            invoiceItemToUpdate.Remove(invoiceItem);
        }

        async Task EditRow(InvoiceItemsVm invoiceItem)
        {
            if (editMode == DataGridEditMode.Single && invoiceItemToInsert.Count() > 0)
            {
                Reset();
            }

            invoiceItemToUpdate.Add(invoiceItem);
            await invoiceItemsGrid.EditRow(invoiceItem);
        }

        void OnUpdateRow(InvoiceItemsVm invoiceItem)
        {
            Reset(invoiceItem);
            int index = InvoiceModal.InvoiceItemList.FindIndex(item => item.Id == invoiceItem.Id);

            if (index != -1)
            {
                // Replace the item at the found index with the updated item
                InvoiceModal.InvoiceItemList[index] = invoiceItem;
            }
        }

        async Task SaveRow(InvoiceItemsVm invoiceItem)
        {
            invoiceItem.UnitPrice = Convert.ToDecimal(invoiceItem.Rate);
            invoiceItem.Description = RemoveHtmlTags(invoiceItem.Description);


            //  await invoiceItemsGrid.UpdateRow(invoiceItem);
        }
        string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
        void CancelEdit(InvoiceItemsVm invoiceItem)
        {
            Reset(invoiceItem);

            invoiceItemsGrid.CancelEditRow(invoiceItem);
            // Find the original item
            var originalItem = InvoiceModal.InvoiceItemList.FirstOrDefault(item => item.Id == invoiceItem.Id);

            if (originalItem != null)
            {
                // Check if the item was modified
                if (!invoiceItem.Equals(originalItem))
                {
                    // Update the edited item with the original values
                    int index = InvoiceModal.InvoiceItemList.IndexOf(invoiceItem);
                    if (index != -1)
                    {
                        InvoiceModal.InvoiceItemList[index] = originalItem;
                    }
                }
            }
        }

        async Task DeleteRow(InvoiceItemsVm invoiceItem)
        {
            Reset(invoiceItem);

            if (InvoiceModal.InvoiceItemList.Contains(invoiceItem))
            {
                InvoiceModal.InvoiceItemList.Remove(invoiceItem);
                await invoiceItemsGrid.Reload();
            }
            else
            {
                invoiceItemsGrid.CancelEditRow(invoiceItem);
                await invoiceItemsGrid.Reload();
            }
        }



        async Task InsertRow()
        {
            if (editMode == DataGridEditMode.Single)
            {
                Reset();
            }

            var invoiceItem = new InvoiceItemsVm();
            invoiceItemToInsert.Add(invoiceItem);
            await invoiceItemsGrid.InsertRow(invoiceItem);


        }

        
        async Task DefaultRows()
        {
            //if (editMode == DataGridEditMode.Single)
            //{
            //    Reset();
            //}
            for (int i = 0; i < 9; i++)
            {
                var invoiceItem = new InvoiceItemsVm();
                invoiceItemToInsert.Add(invoiceItem);
                InvoiceModal.InvoiceItemList.Add(invoiceItem);
                await invoiceItemsGrid.InsertRow(invoiceItem);

            }
        }

        void OnCreateRow(InvoiceItemsVm invoiceItem)
        {
            InvoiceModal.InvoiceItemList.Add(invoiceItem);
            invoiceItemToInsert.Remove(invoiceItem);
        }



        #endregion

        public bool DeleteConfirmationVisibility { get; set; }
        public void OnDeleteConfirmationVisibilityChangedModel(bool visibilityStatus)
        {
            DeleteConfirmationVisibility = visibilityStatus;

        }
        public bool IsDelete { get; set; } = false;
        async Task DeleteInvoice(string id)
        {
            try
            {
                DeleteConfirmationVisibility = true;
                Id = id;
            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
            }

        }

        public string InvoiceId { get; set; }
        private DotNetObjectReference<JobsAttachments> dotNetObjectReference;
        string filename = "Invoice" + "_" + DateTime.Now.ToString("ddmmyyyyhhmmss") + ".pdf";

        public string fileUpload = "fileUploadId";

        public void HandleInvalidSubmit()
        {
            js.InvokeVoidAsync("scrollToValidationMessage");
        }
        
        public async Task SaveData()
        {
            if(InvoiceModal.Total.Value > 0)
            {
                IsSpinner = true;
                await Task.Delay(10);
                ConnectSalesforce();
                // Call the create method to create the record
                try
                {

                    //Modal.InvoiceId = InvoiceId;
                    if (ActionName == "Save")
                    {


                        InvoiceModal.Folder = DateTime.Now.Year.ToString() + "_documents";
                        InvoiceModal.Filename = filename;
                        // create a record using an anonymous class and returns the ID
                        // Add Invoice record first
                        // Create the attachmend PDF
                        string partialName = "/Views/InvoicePdf.cshtml";
                        Items_Copy = InvoiceModal.InvoiceItemList;
                        InvoiceModal.InvoiceItemList = InvoiceModal.InvoiceItemList.Where(x => x.Amount > 0).ToList();
                        htmlContent = RazorRendererHelper.RenderPartialToString(partialName, InvoiceModal);
                        InvoiceModal.InvoiceItemList = Items_Copy;
                        InvoiceId = await IinvoiceServices.AddInvoice(InvoiceModal, InvoiceModal.JobID, htmlContent);
                        foreach (var item in InvoiceModal.InvoiceItemList)
                        {
                            item.InvoiceId = InvoiceId;
                        }
                        Exception registerResponse = await IinvoiceServicesItems.AddInvoiceItem(InvoiceModal.InvoiceItemList);
                        if (registerResponse.Message == "1")
                        {
                            TostModelclass.AlertMessageShow = true;
                            TostModelclass.AlertMessagebody = "Invoice Item Record Added";
                            TostModelclass.Msgstyle = MessageColor.Success;
                            ActionName = "Send Email";

                        }
                        else
                        {

                            TostModelclass.AlertMessageShow = true;
                            TostModelclass.AlertMessagebody = registerResponse.Message;
                            TostModelclass.Msgstyle = MessageColor.Error;

                        }
                    }
                    else if (ActionName == "Send Email")
                    {
                        Modal.Id = Id;
                        Exception registerResponse = await IinvoiceServices.SendInvoiceInEmail(htmlContent, filename, InvoiceModal);
                        if (registerResponse.Message == "1")
                        {
                            TostModelclass.AlertMessageShow = true;
                            TostModelclass.AlertMessagebody = "Invoice Sent to the customer";
                            TostModelclass.Msgstyle = MessageColor.Success;
                        }
                        else
                        {
                            TostModelclass.AlertMessageShow = true;
                            TostModelclass.AlertMessagebody = registerResponse.Message;
                            TostModelclass.Msgstyle = MessageColor.Error;
                        }
                    }


                }
                catch (SalesforceException ex)
                {
                    IsloaderShow = false;
                    TostModelclass.AlertMessageShow = true;
                    TostModelclass.AlertMessagebody = " Failed to save the invoice details : " + ex.Message;
                    TostModelclass.Msgstyle = MessageColor.Error;

                }
                IsSpinner = false;
            } else
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = " Atleast one item is required in the invoice ";
                TostModelclass.Msgstyle = MessageColor.Error;
            }
           

        }



        public async void OnDeleteConfirmationSuccess(bool isAdded)
        {
            if (isAdded)
            {
                DeleteConfirmationVisibility = false;
                Exception registerResponse = await IinvoiceServicesItems.DeleteInvoiceItem(Id);
                if (registerResponse.Message == "1")
                {
                    TostModelclass.AlertMessageShow = true;
                    TostModelclass.AlertMessagebody = "Invoice Item Record Deleted";
                    TostModelclass.Msgstyle = MessageColor.Success;

                }
                //StateHasChanged();
            }
        }

        #region render Pdf Content

        public bool IsPdfPreview { get; set; }

        private string htmlContent;
        [Inject]
        public IRazorRendererHelper RazorRendererHelper { get; set; }
        private async Task LoadHtmlContent()
        {

            // var invoiceItems = await IinvoiceServicesItems.GetInvoiceItems(InvoiceId);
            string partialName = "/Views/InvoicePdf.cshtml";
            Items_Copy = InvoiceModal.InvoiceItemList;
            InvoiceModal.InvoiceItemList = InvoiceModal.InvoiceItemList.Where(x => x.Amount > 0).ToList();
            htmlContent = RazorRendererHelper.RenderPartialToString(partialName, InvoiceModal);
            InvoiceModal.InvoiceItemList = Items_Copy;
            IsPdfPreview = true;
            //htmlContent = await IinvoiceServices.GetInvoiceHtmlContent(baseUrl,InvoiceId);

        }
        public void HidePreviewPdf()
        {
            IsPdfPreview = false;
        }
    }

    #endregion
}

