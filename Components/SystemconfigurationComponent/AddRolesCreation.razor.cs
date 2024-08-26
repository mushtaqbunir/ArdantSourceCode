using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Components.SystemconfigurationComponent
{
    public partial class AddRolesCreation
    {
        [Inject]
        IUsersServices UsersServices { get; set; }
        [Parameter]
        public bool IsVisible { get; set; }
        [Parameter]
        public int RoleId { get; set; } = 0;
        public string UserAvailability = string.Empty;
        public string CssClass = string.Empty;
        public RolesVM RolesVMModal = new();
        public bool IsloaderShow { get; set; } = false;
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        [Parameter]
        public EventCallback<bool> OnAddSuccess { get; set; }
        public PopupDialog PopupDialogs { get; set; } = new();

        [Inject]
        public FGCDbContext Context { get; set; }
        public List<MenuItem> ListOfMenuItem = new();
        public List<ListOfMenuItemFormModel> ListOfUserMenuItem = new();

        protected override async Task OnInitializedAsync()
        {
            ListOfMenuItem = await UsersServices.GetMenuItemList();

        }


        private async Task CheckChanged(ChangeEventArgs ev, int? AllCheckedParentID, string field)
        {

            ListOfMenuItemFormModel Menulist = new();
            var BoolValue = (Boolean)ev.Value;
            Menulist.MenuName = field;
            Menulist.MenuItemParentID = AllCheckedParentID;
            if (BoolValue == true)
            {
                if (!ListOfUserMenuItem.Any(x => x.MenuName == field))
                    ListOfUserMenuItem.Add(Menulist);
                //  await GetMenuItemParentChecked(MenuItemParentID, AllCheckedParentID);//checked
            }
            else
            {
                // await GetMenuItemParentUnChecked(field, AllCheckedParentID);//uncheck 
                ListOfUserMenuItem.RemoveAll(x => x.MenuName == Menulist.MenuName);
            }
            StateHasChanged();

        }
        protected override async Task OnParametersSetAsync()
        {

            if (IsVisible == true && RoleId != 0)
            {
                RolesVMModal = new();

                RolesVMModal = await UsersServices.EditRole(RoleId);
                ListOfUserMenuItem = await UsersServices.GetRolesClaimsList(RoleId);
            }
        }
        public Task CloseSideBar()
        {
            RolesVMModal = new();
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            RolesVMModal = new();
            PopupDialogs.responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public TostModel TostModelclass { get; set; } = new();
        public async Task SaveModel()
        {

            IsloaderShow = true;
            RolesVMModal.ListOfRoleClaims = ListOfUserMenuItem.DistinctBy(x => x.MenuName).ToList();
            var registerResponse = await UsersServices.AddRole(RolesVMModal);
            if (registerResponse.Message == "1" || registerResponse.Message == "2")
            {
                IsloaderShow = false;
                RolesVMModal = new();

                await OnVisibilityChanged.InvokeAsync(false);
                await OnAddSuccess.InvokeAsync(true);
                TostModelclass = registerResponse.Message.AlertSuccessMessage();
            }
            else
            {
                IsloaderShow = false;
                TostModelclass = registerResponse.Message.AlertErrorMessage();
            }
        }
    }
}
