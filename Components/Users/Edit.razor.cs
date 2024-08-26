using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
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
    public partial class Edit
    {
        [Inject]
        IUsersServices UsersServices { get; set; }
        [Parameter]
        public bool IsEditVisible { get; set; }
        [Parameter]
        public int UserID { get; set; }
        [Parameter]

        public EventCallback<bool> OnEditVisibiltyChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnEditSuccess { get; set; }
        public EditVM EditModal = new EditVM();
        public string UserAvailability = string.Empty;
        public string CssClass = string.Empty;
        public bool IsloaderShow { get; set; } = false;
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }

        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public Task CloseSideBar()
        {
            EditModal = new EditVM();
            return OnEditVisibiltyChanged.InvokeAsync(false);
        }

        public List<SelectListItem> lstUserRoles = new();
        public List<SelectListItem> lstUserStatus = new();
        [Inject]
        public FGCDbContext Context { get; set; }
        private List<SelectListItem> GetUserRoles()
        {
            List<SelectListItem>
                userList = (from user in Context.Roles.AsEnumerable()
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
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            EditModal = new EditVM();
            responseDialogVisibility = visibilityStatus;
            return OnEditVisibiltyChanged.InvokeAsync(false);
        }
        protected override Task OnParametersSetAsync()
        {
            if (IsEditVisible && UserID > 0)
            {
                EditModal = UsersServices.GetUserByID(UserID);
                if (EditModal != null)
                {
                    EditModal.UserRole = EditModal.RoleID.ToString();
                }


            }
            return base.OnParametersSetAsync();
        }
        protected override Task OnInitializedAsync()
        {
            try
            {
                lstUserRoles = GetUserRoles();
                GetUserStatus();
                return base.OnInitializedAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateUserData()
        {
            IsloaderShow = true;
            Exception registerResponse = await UsersServices.UpdateUser(EditModal, UserID);
            if (registerResponse.Message == "1")
            {
                IsloaderShow = false;
                await OnEditSuccess.InvokeAsync(true);
                responseHeader = "SUCCESS";
                responseBody = "User Record Updated";
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
            EditModal.Username = EditModal.Email;
            return Task.CompletedTask;
        }
        public void CheckAvailability()
        {
            bool result = UsersServices.CheckAvailability(EditModal.Username);
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
        public TostModel TostModelclass { get; set; } = new();
        public void GenerateUserKey()
        {
            ConfirmOnVisibilityChangedModel(true);
        }
        public void GenerateUserKeyAfterConfirm()
        {
            EditModal.UserKey = Reference.GetUniqueReference($"FGC");
            StateHasChanged();
        }
        public bool ConfirmDialogVisibility { get; set; } = false;
        public async Task ConfirmOnVisibilityChangedModel(bool visibilityStatus)
        {
            responseBody = "Do you want to generate user key ?";
            ConfirmDialogVisibility = visibilityStatus;
          
        }

        public async void OnConfirmBoxSuccess(bool isConfirm)
        {
            if (isConfirm)
            {
                try
                {
                    var msg = "User key generated successfully.";
                    GenerateUserKeyAfterConfirm();
                    await ConfirmOnVisibilityChangedModel(false);
                    TostModelclass = msg.AlertSuccessMessage();
                }
                catch (Exception ex)
                {

                    TostModelclass = ex.Message.ToString().AlertErrorMessage();
                }
             
            }
   
        }
        public void NameChange()
        {
            try
            {
                EditModal.ShortName = "";
                if (!string.IsNullOrEmpty(EditModal.Firstname))
                {
                    EditModal.ShortName = EditModal.Firstname[0].ToString();
                }
                if (!string.IsNullOrEmpty(EditModal.Firstname) && !string.IsNullOrEmpty(EditModal.Lastname))
                {
                    EditModal.ShortName = EditModal.ShortName + EditModal.Lastname[0].ToString();
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
