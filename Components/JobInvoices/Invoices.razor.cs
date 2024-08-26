using ArdantOffical.Data.ModelVm.ClinicalData;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ArdantOffical.Services;
using SalesforceSharp;
using System.Threading.Tasks;
using ArdantOffical.IService;
using System;
using ArdantOffical.Helpers.Enums;
using ArdantOffical.Data.ModelVm.Invoices;
using Radzen.Blazor;
using Radzen;
using ArdantOffical.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;

namespace ArdantOffical.Components.JobInvoices
{
    public partial class Invoices
    {
        [Parameter]
        public bool IsVisible { get; set; }
        [Parameter]
        public string JobId { get; set; }

        private int currentPage = 1;
        private int totalPageQuantity;
        private int totalCount = -1;
        public PaginationDTO paginationObj { get; set; } = new PaginationDTO();
        public InvoicesVMForTable ListOfRecord { get; set; }
        public List<InvoicesVM> lstInvoices { get; set; }
        public InvoicesVM Modal = new InvoicesVM();
        public bool IsloaderShow { get; set; } = false;
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        [Parameter]
        public EventCallback<bool> OnAddSuccess { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }

        public List<SelectListItem> lstStatus = new List<SelectListItem>();
        public TostModel TostModelclass { get; set; } = new();
        [Inject]
        public FGCDbContext Context { get; set; }
        public string ActionName { get; set; } = "Save";


        public Task CloseSideBar()
        {
            Modal = new InvoicesVM();
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            Modal = new InvoicesVM();
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }

        public bool ShowDetailsVisibility { get; set; } = false;
        public string MessageDetail { get; set; } = "";
        public string HeaderTitle { get; set; } = "";
        public async Task ShowMessage(string message, string caption)
        {
            MessageDetail = message;
            HeaderTitle = caption;
            OnShowMessageVisibilityChanged(true);
            StateHasChanged();
        }
        public void OnShowMessageVisibilityChanged(bool visibilityStatus)
        {
            ShowDetailsVisibility = visibilityStatus;
        }

        protected override async Task OnInitializedAsync()
        {
            ConnectSalesforce();
            GetStatus();
           
            GetUoM();
            GetReceipiantEmail();
            await LoadRecords();
        }
        public string ErrorMessage { get; set; }
        public void GetReceipiantEmail()
        {
            ConnectSalesforce();
            try
            {
                var record = SFConnect.client.Query<NDIS>("SELECT Id,PrimaryContactEmail__c  FROM NDIS_Job__c  WHERE Status__c= 'Assigned' AND Id='" + JobId + "' LIMIT 1");
                Modal.SentTo = record.Select(r => r.PrimaryContactEmail__c).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void OnChangeState(ChangeEventArgs e)
        {
            Modal.Status = e.Value.ToString();
        }

        public void GetStatus()
        {
            lstStatus.Add(new SelectListItem() { Value = "Pending", Text = "Pending" });
            lstStatus.Add(new SelectListItem() { Value = "Paid", Text = "Paid" });
            lstStatus.Add(new SelectListItem() { Value = "Overdue", Text = "Overdue" });
            
        }
        public string Id { get; set; }
        public async Task EditInvoice(string id)
        {
            Id = id;
            Modal = await IinvoiceServices.GetInvoiceByID(Id);
            ActionName = "Update";
        }
        async Task LoadRecords(int page = 1, int quantityPerPage = 200)
        {
            try
            {
                currentPage = page;
                paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
                ListOfRecord = await IinvoiceServices.GetInvoices(JobId);
                if (ListOfRecord != null)
                {
                    lstInvoices = ListOfRecord.Invoices;
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

        public async Task SaveData()
        {
            //IsloaderShow = true;
            //ConnectSalesforce();
            //// Call the create method to create the record
            //try
            //{
            //    if (ActionName == "Save")
            //    {
            //        // create a record using an anonymous class and returns the ID
            //        Exception registerResponse =  IinvoiceServices.AddInvoice(Modal, JobId);
            //        if (registerResponse.Message == "1")
            //        {
            //            TostModelclass.AlertMessageShow = true;
            //            TostModelclass.AlertMessagebody = "Invoice Record Saved";
            //            TostModelclass.Msgstyle = MessageColor.Success;
            //            Modal = new InvoicesVM();
            //        }
            //        else
            //        {

            //            TostModelclass.AlertMessageShow = true;
            //            TostModelclass.AlertMessagebody = registerResponse.Message;
            //            TostModelclass.Msgstyle = MessageColor.Error;

            //        }
            //    }
            //    else if (ActionName == "Update")
            //    {
            //        Exception registerResponse = await IinvoiceServices.UpdateInvoice(Modal, Id);
            //        if (registerResponse.Message == "1")
            //        {
            //            TostModelclass.AlertMessageShow = true;
            //            TostModelclass.AlertMessagebody = "Invoice Record Updated";
            //            TostModelclass.Msgstyle = MessageColor.Success;
            //            Modal = new InvoicesVM();
            //        }
            //        else
            //        {
            //            TostModelclass.AlertMessageShow = true;
            //            TostModelclass.AlertMessagebody = registerResponse.Message;
            //            TostModelclass.Msgstyle = MessageColor.Error;
            //        }
            //    }
            //    IsloaderShow = false;
            //    await LoadRecords();
            //}
            //catch (SalesforceException ex)
            //{
            //    IsloaderShow = false;
            //    TostModelclass.AlertMessageShow = true;
            //    TostModelclass.AlertMessagebody = " Failed to save note: " + ex.Message;
            //    TostModelclass.Msgstyle = MessageColor.Error;

            //}

        }

        public async void OnDeleteConfirmationSuccess(bool isAdded)
        {
            //if (isAdded)
            //{
            //    DeleteConfirmationVisibility = false;
            //    Exception registerResponse = await IinvoiceServices.DeleteInvoice(Id);
            //    if (registerResponse.Message == "1")
            //    {
            //        TostModelclass.AlertMessageShow = true;
            //        TostModelclass.AlertMessagebody = "Invoice Record Deleted";
            //        TostModelclass.Msgstyle = MessageColor.Success;
            //        await LoadRecords();
            //    }
            //    //StateHasChanged();
            //}
        }


      
        List<InvoiceItemsVm> invoiceItemsToInsert = new List<InvoiceItemsVm>();
        public List<SelectListItem> ItemsList = new();
        public List<SelectListItem> lstUoM = new();
        InvoiceItemsVm invoiceItemsVm = new InvoiceItemsVm();


        
        private void GetUoM()
        {
            lstUoM.Add(new SelectListItem() { Value = "Minutes", Text = "Minutes" });
            lstUoM.Add(new SelectListItem() { Value = "KM", Text = "KM" });


        }

        #region Add Inovice Item

        public string InvoiceId { get; set; }
        public bool IsInvoiceItem { get; set; }
        public void ShowInvoiceItem(string invoiceid)
        {
            InvoiceId = invoiceid;
            IsInvoiceItem = true;
        }
        public void HideInvoiceItem()
        {
            IsInvoiceItem = false;
        }

        #endregion

    }
}
