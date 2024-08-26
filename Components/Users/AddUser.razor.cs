using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ArdantOffical.Components.Users
{
    public partial class AddUser
    {
        [Inject]
        IUsersServices UsersServices { get; set; }
        [Parameter]
        public bool IsVisible { get; set; }
        public string UserAvailability = string.Empty;
        public string CssClass = string.Empty;
        public AddVM UserModal = new AddVM();
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

        public List<SelectListItem> lstUserRoles = new();
        public List<SelectListItem> lstUserStatus = new();
        [Inject]
        public FGCDbContext Context { get; set; }
        private List<SelectListItem> GetUserRoles()
        {
            List<SelectListItem> userList = (from user in Context.Roles.AsEnumerable()
                                             select new SelectListItem
                                             {
                                                 Text = user.Name,
                                                 Value = user.Id.ToString()
                                             }).ToList();
            //Add Default Item at First Position.
            userList.Insert(0, new SelectListItem { Text = "--Select User Role--", Value = "0" });
            return userList;
        }
        private void GetUserStatus()
        {

            lstUserStatus.Add(new SelectListItem() { Value = "1", Text = "Active" });
            lstUserStatus.Add(new SelectListItem() { Value = "0", Text = "Blocked" });
            lstUserStatus.Insert(0, new SelectListItem { Text = "--Select Status--", Value = "3" });
        }
        protected override Task OnInitializedAsync()
        {
            lstUserRoles = GetUserRoles();
            GetUserStatus();
            return Task.CompletedTask;
        }
        public Task CloseSideBar()
        {
            UserModal = new AddVM();
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            UserModal = new AddVM();
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public async Task SaveUserData()
        {
            IsloaderShow = true;
            Exception registerResponse = await UsersServices.SaveUser(UserModal);
            if (registerResponse.Message == "1")
            {
                IsloaderShow = false;
                await OnAddSuccess.InvokeAsync(true);
                responseHeader = "SUCCESS";
                responseBody = "User Record Saved";
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

        public Task UseEmailAsUsername()
        {
            UserModal.Username = UserModal.Email;
            return Task.CompletedTask;
        }

        public void CheckAvailability()
        {
            bool result = UsersServices.CheckAvailability(UserModal.Username);
            if (!result)
            {
                CssClass = "text-success";
                UserAvailability = "Username is available";
            }
            else
            {
                CssClass = "text-danger";
                UserAvailability = "Username is not available";
            }
        }
        //public void GenerateUserKey()
        //{
        //    UserModal.UserKey = Reference.GetUniqueReference($"FGC");
        //    StateHasChanged();
        //}
        public void NameChange()
        {
            try
            {
                UserModal.ShortName = "";
                if (!string.IsNullOrEmpty(UserModal.Firstname))
                {
                    UserModal.ShortName = UserModal.Firstname[0].ToString();
                }
                if (!string.IsNullOrEmpty(UserModal.Firstname) &&!string.IsNullOrEmpty(UserModal.Lastname))
                {
                    UserModal.ShortName = UserModal.ShortName + UserModal.Lastname[0].ToString();
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
