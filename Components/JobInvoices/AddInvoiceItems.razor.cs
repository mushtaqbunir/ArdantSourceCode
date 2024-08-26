using ArdantOffical.Data.ModelVm.Invoices;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data;
using ArdantOffical.Helpers.Enums;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesforceSharp;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Radzen.Blazor;
using Radzen;
using System.Text.RegularExpressions;

namespace ArdantOffical.Components.JobInvoices
{
    public partial class AddInvoiceItems
    {
        [Parameter]
        public bool IsVisible { get; set; }
        [Parameter]
        public string InvoiceId { get; set; }

        private int currentPage = 1;
        private int totalPageQuantity;
        private int totalCount = -1;
        public PaginationDTO paginationObj { get; set; } = new PaginationDTO();
        public InvoiceItemsVm lstInvoiceItem { get; set; } = new();
        public List<InvoiceItemsVm> lstInvoiceItems { get; set; }
        public InvoiceItemsVm Modal = new InvoiceItemsVm();
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
        public List<SelectListItem> lstAssignedJobs = new List<SelectListItem>();
        
        public TostModel TostModelclass { get; set; } = new();
        [Inject]
        public FGCDbContext Context { get; set; }
        public string ActionName { get; set; } = "Save";


        public Task CloseSideBar()
        {
            Modal = new InvoiceItemsVm();
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            Modal = new InvoiceItemsVm();
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
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ConnectSalesforce();
                GetItems();
                GetUoM();
                await LoadAssignedJobs();
                await LoadRecords();
            }
        }
        public string ErrorMessage { get; set; }
      
       
        public string Id { get; set; }
        public async Task EditInvoice(string id)
        {
            Id = id;
            Modal = await IinvoiceServicesItems.GetInvoiceByID(Id);
            ActionName = "Update";
        }
        async Task LoadRecords(int page = 1, int quantityPerPage = 200)
        {
            try
            {
                currentPage = page;
                paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
                lstInvoiceItem = await IinvoiceServicesItems.GetInvoiceItem(InvoiceId);
                //ActionName = lstInvoiceItem.InvoiceItemList.Count > 0 ? "Add More" : "Add";
                if (lstInvoiceItem != null)
                {
                    lstInvoiceItems = lstInvoiceItem.InvoiceItemList;
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

        public async Task LoadAssignedJobs()
        {
            lstAssignedJobs = await IinvoiceServicesItems.GetAssignedJobs();
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
            IsloaderShow = true;
            ConnectSalesforce();
            // Call the create method to create the record
            try
            {
                Modal.InvoiceId = InvoiceId;
                if (ActionName == "Save")
                {
                    // create a record using an anonymous class and returns the ID
                    
                    Exception registerResponse = await IinvoiceServicesItems.AddInvoiceItem(Modal);
                    if (registerResponse.Message == "1")
                    {
                        TostModelclass.AlertMessageShow = true;
                        TostModelclass.AlertMessagebody = "Invoice Item Record Added";
                        TostModelclass.Msgstyle = MessageColor.Success;
                        Modal = new InvoiceItemsVm();
                    }
                    else
                    {

                        TostModelclass.AlertMessageShow = true;
                        TostModelclass.AlertMessagebody = registerResponse.Message;
                        TostModelclass.Msgstyle = MessageColor.Error;

                    }
                }
                else if (ActionName == "Update")
                {
                    Modal.Id = Id;
                    Exception registerResponse = await IinvoiceServicesItems.UpdateInvoiceItem(Modal);
                    if (registerResponse.Message == "1")
                    {
                        TostModelclass.AlertMessageShow = true;
                        TostModelclass.AlertMessagebody = "Invoice Item Record Updated";
                        TostModelclass.Msgstyle = MessageColor.Success;
                        Modal = new InvoiceItemsVm();
                    }
                    else
                    {
                        TostModelclass.AlertMessageShow = true;
                        TostModelclass.AlertMessagebody = registerResponse.Message;
                        TostModelclass.Msgstyle = MessageColor.Error;
                    }
                }
                IsloaderShow = false;
                await LoadRecords();
            }
            catch (SalesforceException ex)
            {
                IsloaderShow = false;
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = " Failed to save note: " + ex.Message;
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
                    await LoadRecords();
                }
                //StateHasChanged();
            }
        }



        List<InvoiceItemsVm> invoiceItemsToInsert = new List<InvoiceItemsVm>();
        public List<SelectListItem> ItemsList = new();
        public List<SelectListItem> lstUoM = new();
        InvoiceItemsVm invoiceItemsVm = new InvoiceItemsVm();


        private void GetItems()
        {
            ItemsList.Add(new SelectListItem() { Value = "NDIS Travel ", Text = "NDIS Travel" });
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

        #region Radzden Data Grid

        RadzenDataGrid<InvoiceItemsVm> invoiceItemGrid;
        //IEnumerable<Order> orders;

        DataGridEditMode editMode = DataGridEditMode.Single;

        List<InvoiceItemsVm> InvoiceItemsToInsert = new List<InvoiceItemsVm>();
        List<InvoiceItemsVm> InvoiceItemsToUpdate = new List<InvoiceItemsVm>();

        void Reset()
        {
            InvoiceItemsToInsert.Clear();
            InvoiceItemsToUpdate.Clear();
        }

        void Reset(InvoiceItemsVm invoiceItem)
        {
            InvoiceItemsToInsert.Remove(invoiceItem);
            InvoiceItemsToUpdate.Remove(invoiceItem);
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    await base.OnInitializedAsync();

        //    customers = dbContext.Customers;
        //    employees = dbContext.Employees;

        //    orders = dbContext.Orders.Include("Customer").Include("Employee");
        //}

        async Task EditRow(InvoiceItemsVm invoiceItem)
        {
            if (editMode == DataGridEditMode.Single && InvoiceItemsToInsert.Count() > 0)
            {
                Reset();
            }

            InvoiceItemsToUpdate.Add(invoiceItem);
            await invoiceItemGrid.EditRow(invoiceItem);
        }

        async void OnUpdateRow(InvoiceItemsVm invoiceItem)
        {
            Reset(invoiceItem);
          
            //dbContext.Update(order);

            //dbContext.SaveChanges();
        }

        async Task SaveRow(InvoiceItemsVm invoiceItem)
        {
            RemoveHtmlTags(invoiceItem.Description);
            await invoiceItemGrid.UpdateRow(invoiceItem);
        }

        void CancelEdit(InvoiceItemsVm invoiceItem)
        {
            Reset(invoiceItem);

            invoiceItemGrid.CancelEditRow(invoiceItem);

            //var orderEntry = dbContext.Entry(order);
            //if (orderEntry.State == EntityState.Modified)
            //{
            //    orderEntry.CurrentValues.SetValues(orderEntry.OriginalValues);
            //    orderEntry.State = EntityState.Unchanged;
            //}
        }

        async Task DeleteRow(InvoiceItemsVm invoiceItem)
        {
            Reset(invoiceItem);
          await  invoiceItemGrid.Reload();
            //if (orders.Contains(order))
            //{
            //    dbContext.Remove<Order>(order);

            //    dbContext.SaveChanges();

            //    await ordersGrid.Reload();
            //}
            //else
            //{
            //    ordersGrid.CancelEditRow(order);
            //    await ordersGrid.Reload();
            //}
        }

        async Task InsertRow()
        {
            if (editMode == DataGridEditMode.Single)
            {
                Reset();
            }

            var invoiceItem = new InvoiceItemsVm();
            InvoiceItemsToInsert.Add(invoiceItem);
          
            await invoiceItemGrid.InsertRow(invoiceItem);
        }

        void OnCreateRow(InvoiceItemsVm invoiceItem)
        {
            invoiceItemsToInsert.Add(invoiceItem);
            //dbContext.Add(order);

            //dbContext.SaveChanges();

            //ordersToInsert.Remove(order);
        }
        public string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, @"<[^>]+>|&nbsp;", string.Empty);
        }

        #endregion
    }
}
