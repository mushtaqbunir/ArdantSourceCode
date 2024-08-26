using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArdantOffical.Pages
{
    public partial class NavMenuLinkComponent
    {
        [Inject]
        IUsersServices UsersServices { get; set; }
        private int currentPage = 1;
        private int totalPageQuantity;
        private int totalCount = -1;
        public PaginationDTO paginationObj { get; set; } = new PaginationDTO();
        public MenuItemUseForTable ListOfRecord { get; set; }
        public List<MenuItemVM> PerPagMenuItemRecords { get; set; }
        public List<MenuItemVM> SearchMenuItemRecordsList { get; set; }
        /*   Modal Popup Params */
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public int UserID { get; set; }
        public CurrentUserInfoVM Userinfo { get; set; }
        [Inject]
        public AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        // Download File name
        private string fileName = "Users" + DateTime.Now.ToString("ddMMyyyyhhmmss");
        public bool ModalShowpopupVisibility { get; set; } = false;
        public bool AddSideBarVisibility { get; set; } = false;

        public bool ChangePasswordSideBarVisibility { get; set; } = false;
        public void OnAddUserVisibilityChanged(bool visibilityStatus)
        {
            AddSideBarVisibility = visibilityStatus;
        }

        public void OnPasswordVisibiltyChanged(bool visibilityStatus)
        {
            ChangePasswordSideBarVisibility = visibilityStatus;
        }
        public async void OnAddSuccess(bool isAdded)
        {
            await LoadRecords();
        }
        public async void OnEditSuccess()
        {
            await LoadRecords();
        }
        public async void OnPasswordChangedSuccess()
        {
            await LoadRecords();
        }
        public void ShowAddNewLinkSideBar()
        {
            OnAddUserVisibilityChanged(true);
        }
        //MenuAccess
        public bool MenuAccessSideBarVisibility { get; set; } = false;
        public int UserId { get; set; }
        public void ShowMenuAccessSideBar(int id)
        {
            UserId = id;
            OnEditMenuAccessVisibilityChanged(true);
        }
        public void OnEditMenuAccessVisibilityChanged(bool visibilityStatus)
        {
            MenuAccessSideBarVisibility = visibilityStatus;
        }
        public async void OnMenuAccessSuccess()
        {
            await LoadRecords();
        }
        public bool EditSideBarVisibility { get; set; } = false;
        public void OnEditVisibilityChanged(bool visibilityStatus)
        {
            AddSideBarVisibility = visibilityStatus;
        }
        public int MenuItemId { get; set; } = 0;
        public void ShowEditSideBar(int id)
        {
            MenuItemId = id;
            OnEditVisibilityChanged(true);
        }

        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        // Delete Confirmation

        public bool IsDelete { get; set; } = false;

        private async Task SelectedPage(int page)
        {
            currentPage = page;
            await LoadRecords(page);
        }
        private HubConnection? hubConnection;
        [Inject]
        public FGCDbContext Context { get; set; }
        async Task LoadRecords(int page = 1, int quantityPerPage = 25)
        {
            try
            {
                currentPage = page;
                paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
                ListOfRecord = await UsersServices.GetMenuItemNavMenuLink(paginationObj);
                if (ListOfRecord != null)
                {
                    PerPagMenuItemRecords = ListOfRecord.MenuItemList;

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

        public List<SelectListItem> ListOfTablePages = new List<SelectListItem>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                ListOfTablePages.Add(new SelectListItem() { Text = "50", Value = "50" });
                ListOfTablePages.Add(new SelectListItem() { Text = "75", Value = "75" });
                ListOfTablePages.Add(new SelectListItem() { Text = "100", Value = "100" });
                paginationObj = new PaginationDTO();
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
        public string SearchFilter { get; set; }
        private async Task SearchChanged()
        {


            if (!string.IsNullOrEmpty(SearchKey))
            {
                SearchKey = SearchKey.ToLower();
                try
                {
                    ListOfRecord = await UsersServices.GetMenuItemNavMenuLink(paginationObj, SearchKey);
                    if (ListOfRecord != null)
                    {
                        PerPagMenuItemRecords = ListOfRecord.MenuItemList;
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
            StateHasChanged();
        }
        private string _searchKey;
        public string SearchKey
        {
            get { return _searchKey; }
            set
            {
                if (_searchKey != value)
                {
                    _searchKey = value;
                    //if (string.IsNullOrWhiteSpace(value))
                    //    SearchString2(value);
                }
            }
        }
        private async Task FormReset()
        {
            PerPagMenuItemRecords = new();
            SearchKey = "";
            await LoadRecords(1, paginationObj.QuantityPerPage);
            StateHasChanged();
        }
        async Task DeleteMenuItem(int id)
        {
            try
            {
                DeleteConfirmationVisibility = true;
                MenuItemId = id;
            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
            }

        }

        // Delete Confirmation
        public bool DeleteConfirmationVisibility { get; set; }
        public void OnDeleteConfirmationVisibilityChangedModel(bool visibilityStatus)
        {
            DeleteConfirmationVisibility = visibilityStatus;

        }
        //public bool IsDelete { get; set; } = false;
        public async void OnDeleteConfirmationSuccess(bool isAdded)
        {
            if (isAdded)
            {
                DeleteConfirmationVisibility = false;
                Exception registerResponse = await UsersServices.DeleteMenuItem(MenuItemId);
                if (registerResponse.Message == "1")
                {
                    responseHeader = "SUCCESS";
                    responseBody = "Record Deleted";
                    responseDialogVisibility = true;
                    await LoadRecords();
                }
                StateHasChanged();
            }
        }
    }
}
