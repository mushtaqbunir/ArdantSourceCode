using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Pages
{
    public partial class RolesCreation
    {
        [Inject]
        IUsersServices UsersServices { get; set; }
        private int currentPage = 1;

        public PaginationDTO paginationObj { get; set; } = new PaginationDTO();
        public GenericVMUseForTable<RolesVM> ListOfRecord { get; set; } = new();

        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }


        public PopupDialog PopupDialogs { get; set; }

        public int UserID { get; set; }
        public CurrentUserInfoVM Userinfo { get; set; }
        [Inject]
        public AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        // Download File name
        private string fileName = "Users" + DateTime.Now.ToString("ddMMyyyyhhmmss");
        public bool ModalShowpopupVisibility { get; set; } = false;
        public bool AddSideBarVisibility { get; set; } = false;
        public TostModel TostModelclass { get; set; } = new();
        public bool EditSideBarVisibility { get; set; } = false;
        public bool ChangePasswordSideBarVisibility { get; set; } = false;
        public void OnAddVisibilityChanged(bool visibilityStatus)
        {
            AddSideBarVisibility = visibilityStatus;
        }
        public void OnEditUserVisibilityChanged(bool visibilityStatus)
        {
            EditSideBarVisibility = visibilityStatus;
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
        public void ShowAddUserSideBar()
        {
            RoleId = 0;
            OnAddVisibilityChanged(true);
        }
        //MenuAccess
        public bool MenuAccessSideBarVisibility { get; set; } = false;
        public int UserId { get; set; }
        public void ShowMenuAccessSideBar(int id)
        {
            RoleId = id;
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
        public void ShowEditUserSideBar(int id)
        {
            UserID = id;
            OnEditUserVisibilityChanged(true);
        }
        public void ShowChangePasswordSideBar(int id)
        {
            UserID = id;
            OnPasswordVisibiltyChanged(true);
        }

        // Delete Confirmation
        public bool DeleteConfirmationVisibility { get; set; }
        public void OnDeleteConfirmationVisibilityChangedModel(bool visibilityStatus)
        {
            DeleteConfirmationVisibility = visibilityStatus;

        }
        public bool IsDelete { get; set; } = false;
        public async void OnDeleteConfirmationSuccess(bool isAdded)
        {

            if (isAdded)
            {
                PopupDialogs = new();
                DeleteConfirmationVisibility = false;
                Exception registerResponse = await UsersServices.DeleteRole(RoleId);
                if (registerResponse.Message == "3")
                {
                    //PopupDialogs.responseHeader = "SUCCESS";
                    //PopupDialogs.responseBody = "User Record Deleted";
                    //PopupDialogs.responseDialogVisibility = true;
                    TostModelclass = registerResponse.Message.AlertSuccessMessage();
                    await LoadRecords();
                }
                else
                {
                    TostModelclass = registerResponse.Message.AlertErrorMessage();
                }
                StateHasChanged();
            }
        }
        private async Task SelectedPage(int page)
        {
            currentPage = page;
            await LoadRecords(page);
        }
        private HubConnection? hubConnection;
        [Inject]
        public FGCDbContext Context { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        // public static  List<SystemActiveUsers> CurrentObjActiveList = new List<SystemActiveUsers>();
        [Inject]
        public IServiceScopeFactory _serviceScope { get; set; }
        public bool IsloaderShow { get; set; } = false;
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            PopupDialogs.responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public string GetUserRole(int RoleID)
        {
            try
            {
                using (IServiceScope scope = _serviceScope.CreateScope()) // this will use `IServiceScopeFactory` internally
                {
                    FGCDbContext contextt = scope.ServiceProvider.GetService<FGCDbContext>();
                    return (from x in contextt.Roles
                            where x.Id == RoleID
                            select x.Name).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        async Task LoadRecords(int page = 1, int quantityPerPage = 25)
        {
            try
            {
                currentPage = page;
                paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
                ListOfRecord = await UsersServices.GetRoles(paginationObj);

            }
            catch (Exception ex)
            {
                PopupDialogs = new();
                PopupDialogs.responseHeader = "ERROR";
                PopupDialogs.responseBody = ex.Message;
                PopupDialogs.responseDialogVisibility = true;
                throw;
            }
        }
        async Task DeleteModel(int id)
        {
            try
            {
                DeleteConfirmationVisibility = true;
                RoleId = id;
            }
            catch (Exception ex)
            {
                PopupDialogs = new();
                PopupDialogs.responseHeader = "ERROR";
                PopupDialogs.responseBody = ex.Message;
                PopupDialogs.responseDialogVisibility = true;

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
                PopupDialogs = new();
                ListOfRecord = new();
                await LoadRecords();
                Userinfo = await UserauthenticationStateProvider.CurrentUser();
            }
            catch (Exception ex)
            {
                PopupDialogs = new();
                PopupDialogs.responseHeader = "ERROR";
                PopupDialogs.responseBody = ex.Message;
                PopupDialogs.responseDialogVisibility = true;
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
                    currentPage = 1;
                    paginationObj = new PaginationDTO() { Page = paginationObj.Page, QuantityPerPage = paginationObj.QuantityPerPage };
                    ListOfRecord = await UsersServices.GetRoles(paginationObj, SearchKey);

                }
                catch (Exception ex)
                {
                    PopupDialogs = new();
                    PopupDialogs.responseHeader = "ERROR";
                    PopupDialogs.responseBody = ex.Message;
                    PopupDialogs.responseDialogVisibility = true;
                    throw;
                }
            }
            else
            {
                await LoadRecords(1, paginationObj.QuantityPerPage);
            }
            // await LoadPeople();
            StateHasChanged();
        }
        private string _searchKey;
        public string SearchKey
        {
            get => _searchKey;
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

            SearchKey = "";
            await LoadRecords(1, paginationObj.QuantityPerPage);
            StateHasChanged();
        }
        public int RoleId { get; set; } = 0;
        public void ShowEditSideBar(int id)
        {
            RoleId = id;
            OnEditVisibilityChanged(true);
        }
        public void OnEditVisibilityChanged(bool visibilityStatus)
        {
            AddSideBarVisibility = visibilityStatus;
        }
        public async void OnAddRoleSuccess(bool isAdded)
        {

        }
    }
}
