using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        public IUsersServices UsersServices { get; set; }
        [Parameter]
        public EventCallback LoadProfilePicture { get; set; }
        public int DBPID { get; set; }
        public bool responseDialogVisibility { get; set; } = false;
        public string responseHeader { get; set; } = "Logout";
        public string responseBody { get; set; } = "Are you sure to logout";
        private DotNetObjectReference<LoginDisplay> dotNetObjectReference;
        public List<AttachmentsVm> attachments = new List<AttachmentsVm>();
        public void OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;

        }

        protected override async Task OnInitializedAsync()
        {
            userInfo = await UserauthenticationStateProvider.CurrentUser();
           await LoadProfilePic();
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                dotNetObjectReference = DotNetObjectReference.Create(this);
                //await js.InvokeAsync<string>("createObjectForLogOut", dotNetObjectReference);
                //await js.InvokeAsync<string>("SetIdleTimeLogOut");
                await js.InvokeVoidAsync("setupPopover");

            }
           
            
        }
        [JSInvokable]
        public async Task LogOutIdle()
        {
            //navigationManager.NavigateTo("/Identity/Account/LogOut", true);
            //StateHasChanged();
        }
        public async void OnDeleteActionSuccess(bool isAdded)
        {
            if (isAdded)
            {

                //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                //await js.InvokeVoidAsync("NavigateToUrl", "/Identity/Account/LogOut", "_parent");
                //navigationManager.NavigateTo("/Identity/Account/LogOut", true);
                await js.InvokeVoidAsync("eval", "document.querySelector('a[href=\"/Identity/Account/Logout\"]').click();");
                //DepartmentList = await Depart.GetDepartments();
                StateHasChanged();

            }
        }
        public bool ChangePasswordSideBarVisibility { get; set; } = false;
        [Inject]
        public AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        public void OnPasswordVisibiltyChanged(bool visibilityStatus)
        {
            ChangePasswordSideBarVisibility = visibilityStatus;
        }
        public int UserID { get; set; }
        CurrentUserInfoVM userInfo = new CurrentUserInfoVM();

        public async Task ShowChangePasswordSideBar(int id)
        {
            CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
            IsProfileView = false;
            if (Userinfo != null)
            {
                UserID = Userinfo.UserId;
            }
            OnPasswordVisibiltyChanged(true);
        }
        public async void OnPasswordChangedSuccess()
        {
            //await LoadRecords();
        }

       

        #region profile View
        public string SFUserId { get; set; }
        public bool IsProfileView { get; set; }
        public void ProfileView(string id)
        {
            ChangePasswordSideBarVisibility = false;
            SFUserId = id;
            IsProfileView = !IsProfileView;
        }
        public void ProfileViewHide()
        {
            IsProfileView = false;
        }
        #endregion

        #region Refresh Header

      
        public async Task LoadProfilePic()
        {
            attachments = await UsersServices.GetOT_ProfilePic(userInfo.SalesforceID);
        }
        private async void RefreshHeaderMenu()
        {
          await  LoadProfilePic();
        }

        #endregion


        #region Edit Admin
        public bool EditSideBarVisibility { get; set; } = false;
        public void ShowEditUserSideBar(int id)
        {
            UserID = id;
            OnEditUserVisibilityChanged(true);
        }
        public void OnEditUserVisibilityChanged(bool visibilityStatus)
        {
            EditSideBarVisibility = visibilityStatus;
        }
        public async void OnEditSuccess()
        {
            
        }
        #endregion
    }
}
