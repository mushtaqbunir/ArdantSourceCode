using ArdantOffical.Data.ModelVm.Users;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ArdantOffical.Components.Users
{
    public partial class ChangePassword
    {
        [Parameter]
        public bool IsChangePasswordPopupVisibile { get; set; }
        [Parameter]
        public int UserID { get; set; }
        [Parameter]

        public EventCallback<bool> OnPasswordVisibiltyChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnPasswordChangedSuccess { get; set; }
        public ChangePassVM PasswordModal = new ChangePassVM();
        public bool IsloaderShow { get; set; } = false;
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }

        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public Task CloseSideBar()
        {
            PasswordModal = new ChangePassVM();
            return OnPasswordVisibiltyChanged.InvokeAsync(false);
        }


        protected override Task OnParametersSetAsync()
        {
            if (IsChangePasswordPopupVisibile && UserID > 0)
            {
                currentTxt = false;
                currentTxtType = "password";
                currentTxtTypeIcon = "fa fa-eye-slash";
                newTxt = false;
                newTxtTypeIcon = "fa fa-eye-slash";
                newTxtType = "password";
                confirmTxt = false;
                confirmTxtTypeIcon = "fa fa-eye-slash";
                confirmTxtType = "password";
                PasswordModal = IuserServices.GetUserPasswordByID(UserID);
            }
            return base.OnParametersSetAsync();
        }
        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            PasswordModal = new ChangePassVM();
            responseDialogVisibility = visibilityStatus;
            return OnPasswordVisibiltyChanged.InvokeAsync(false);
        }

        public async Task UpdatePassword()
        {
            IsloaderShow = true;
            Exception registerResponse = await IuserServices.ChangePassword(PasswordModal, UserID);
            if (registerResponse.Message == "1")
            {
                IsloaderShow = false;
                PasswordModal = new();
                await OnPasswordChangedSuccess.InvokeAsync(true);
                responseHeader = "SUCCESS";
                responseBody = "Password changed";
                responseDialogVisibility = true;
            }
            else
            {
                IsloaderShow = false;
                responseHeader = "ERROR";
                responseBody = registerResponse.Message;
                responseDialogVisibility = true;
            }
        }
    }
}
