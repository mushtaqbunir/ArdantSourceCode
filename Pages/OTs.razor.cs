using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data;
using ArdantOffical.Helpers.Enums;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.SignalRHub;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Models;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ArdantOffical.Pages
{
    public partial class OTs
    {
        [Inject]
        IUsersServices UsersServices { get; set; }
        private int currentPage = 1;
        private int totalPageQuantity;
        private int totalCount = -1;
        public PaginationDTO paginationObj { get; set; } = new PaginationDTO();
        public UserVmUseForTable ListOfRecord { get; set; }
        public List<UserVM> PerPageUserRecords { get; set; }
        public IQueryable<UserVM> PerPageUserRecordIQueryable { get; set; }
        public List<UserVM> AllUserRecords { get; set; }
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
        public bool EditSideBarVisibility { get; set; } = false;
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
        public void ShowAddUserSideBar()
        {
            OnAddUserVisibilityChanged(true);
        }
        public TostModel TostModelclass { get; set; } = new();
        public bool IsloaderShow { get; set; } = false;
        public string IsloaderShow33 { get; set; }
        async Task OnChange(bool? value, int id)
        {

            var registerResponse = await UsersServices.EnableTwoFactorGoogleAuthenticator(id);
            if (registerResponse.Message == "1" || registerResponse.Message == "2")
            {
                IsloaderShow = false;
                var Message = "Google authentication is been Successfully Disable";
                TostModelclass = Message.AlertSuccessMessage();
            }
            else
            {
                IsloaderShow = false;
                TostModelclass = registerResponse.Message.AlertErrorMessage();
            }
        }
        public async Task EnableTwoFactorGA(int id)
        {
            var registerResponse = await UsersServices.EnableTwoFactorGoogleAuthenticator(id);
            if (registerResponse.Message == "1" || registerResponse.Message == "2")
            {
                IsloaderShow = false;
                var Message = "Google authentication is been Successfully Disable";
                TostModelclass = Message.AlertSuccessMessage();
            }
            else
            {
                IsloaderShow = false;
                TostModelclass = registerResponse.Message.AlertErrorMessage();
            }
        }

        async Task OnSkipChange(bool? value, int id)
        {

            var registerResponse = await UsersServices.SkipGoogleAuthenticator(id, value.Value);
            if (registerResponse.Message == "1" || registerResponse.Message == "2")
            {
                IsloaderShow = false;
                var Message = string.Empty;
                if (value.Value)
                    Message = "Google Authentication feature is disabled for the selected user account";
                else
                    Message = "Google Authentication feature is enabled for the selected user account";
                //TostModelclass = Message.AlertSuccessMessage();
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = Message;
                TostModelclass.Msgstyle = MessageColor.Success;
            }
            else
            {
                IsloaderShow = false;
                TostModelclass = registerResponse.Message.AlertErrorMessage();
            }
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

        public void ShowChangePasswordSideBar(int id)
        {
            UserID = id;
            OnPasswordVisibiltyChanged(true);
        }
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }

        // Delete Confirmation
        public bool DeleteConfirmationVisibility { get; set; }
        public void OnDeleteConfirmationVisibilityChangedModel(bool visibilityStatus)
        {
            DeleteConfirmationVisibility = visibilityStatus;

        }
        public bool IsDelete { get; set; } = false;
        public string SalesforceID { get; set; }
        async Task DeleteUser(string salesforceId)
        {
            try
            {
                DeleteConfirmationVisibility = true;
                SalesforceID = salesforceId;
            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
            }

        }
        public async void OnDeleteConfirmationSuccess(bool isAdded)
        {
            if (isAdded)
            {
                DeleteConfirmationVisibility = false;
                Exception registerResponse = await UsersServices.DeleteOT(SalesforceID);
                if (registerResponse.Message == "1")
                {
                    responseHeader = "SUCCESS";
                    responseBody = "User Record Deleted";
                    responseDialogVisibility = true;
                    await LoadRecords();
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
     

        async Task LoadRecords(int page = 1, int quantityPerPage = 200)
        {
            try
            {
                currentPage = page;
                paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
                ListOfRecord = await UsersServices.GetAllOTs(paginationObj);
                if (ListOfRecord != null)
                {
                    PerPageUserRecords = ListOfRecord.UserVmList;
                    AllUserRecords = ListOfRecord.User_Export;
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
                ListOfTablePages.Add(new SelectListItem() { Text = "200", Value = "200" });
                hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/chatHub"), opt =>
                {
                    if (htp.HttpContext.Request.Cookies.Count > 0)
                    {
                        opt.Cookies.Add(new Uri(NavigationManager.BaseUri), new System.Net.Cookie("RememberMeBlogAcademy", htp.HttpContext.Request.Cookies.Where(s => s.Key == "RememberMeBlogAcademy").FirstOrDefault().Value));
                    }
                }).Build();
                await hubConnection.StartAsync();

                hubConnection.On<List<ActiveListUser>>("ReceiveMessage", (List<ActiveListUser> ObjActiveList) =>
                {
                    //var CurrenxtActiveList =;
                    // CurrentObjActiveList = new List<ActiveListUser>();
                    //StateHasChanged();
                });



                paginationObj = new PaginationDTO();
                paginationObj.QuantityPerPage = 200;
                // await hubConnection.InvokeAsync("SendMessage");
                await LoadRecords();
                Userinfo = await UserauthenticationStateProvider.CurrentUser();
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
                    currentPage = 1;
                    paginationObj = new PaginationDTO() { Page = paginationObj.Page, QuantityPerPage = paginationObj.QuantityPerPage };
                    ListOfRecord = await UsersServices.GetUserSearch(paginationObj, SearchKey);
                    if (ListOfRecord != null)
                    {
                        PerPageUserRecords = ListOfRecord.UserVmList;
                        AllUserRecords = ListOfRecord.User_Export;
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

        #region Edit Profile
        public void ShowEditUserSideBar(int id)
        {
            UserID = id;
            OnEditUserVisibilityChanged(true);
        }
        public void OnEditUserVisibilityChanged(bool visibilityStatus)
        {
            EditSideBarVisibility = visibilityStatus;
        }
        #endregion


        private async Task FormReset()
        {
            PerPageUserRecords = new();
            SearchKey = "";
            await LoadRecords(1, paginationObj.QuantityPerPage);
            StateHasChanged();
        }
    }
}
