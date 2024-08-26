using FGCCore.Data;
using FGCCore.Data.ModelVm.Users;
using FGCCore.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FGCCore.Components.Users
{
    public partial class EditUser
    {
        [Parameter]
        public bool IsEditVisible { get; set; }
        [Parameter]
        public int UserID { get; set; }
        [Parameter]
        public EventCallback<bool> OnEditVisibiltyChanged { get; set; }
        [Parameter]
        public EventCallback<bool> OnEditSuccess { get; set; }
        public string UserAvailability = string.Empty;
        public string CssClass = string.Empty;
        public Data.ModelVm.Users.UserVM UserModal = new UserVM();

        readonly IUsersServices IuserService;

        public bool IsloaderShow { get; set; } = false;
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }

        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }

        public List<SelectListItem>
            lstUserRoles = new();
        [Inject]
        public FGCDbContext Context { get; set; }
        private List<SelectListItem>
            GetUserRoles()
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


        protected async Task OnParametersSetAsync()
        {
            if (IsEditVisible)
            {
                // Load User data

            }
        }

        protected async Task OnInitializedAsync()
        {
            lstUserRoles = GetUserRoles();
            // UserModal = IuserService.GetUserByID(UserID);
        }

        public Task CloseSideBar()
        {
            UserModal = new UserVM();
            return OnEditVisibiltyChanged.InvokeAsync(false);
        }


        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            //  UserModal = new UserVM();
            responseDialogVisibility = visibilityStatus;
            return OnEditVisibiltyChanged.InvokeAsync(false);
        }
        public async Task UpdateUserData()
        {
            IsloaderShow = true;
            var registerResponse = await IuserService.UpdateUser(UserModal, UserID);
            if (registerResponse.Message == "1")
            {
                IsloaderShow = false;
                await OnEditSuccess.InvokeAsync(true);
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
            bool result = IuserService.CheckAvailability(UserModal.Username);
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
    }
}
