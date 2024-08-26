using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Components.SystemconfigurationComponent
{
    public partial class AddNavMenuLink
    {
        [Parameter]
        public bool IsVisible { get; set; }
        [Parameter]
        public int MenuItemId { get; set; } = 0;
        [Parameter]
        public int MenuItemParentID { get; set; } = 0;


        public string UserAvailability = string.Empty;
        public string CssClass = string.Empty;
        public MenuItemVM MenuItemModal = new();
        public MenuItem MenuItemModalOrg = new();
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
        public List<MenuItem> MenuItemList { get; set; }

        public List<SelectListItem> GrantParentList = new();
        public List<SelectListItem> ParentList = new();
        public List<SelectListItem> ChildList = new();
        [Inject]
        public FGCDbContext Context { get; set; }
        public async Task MenuitemParentlistFill(int? id)
        {
            MenuItemModal.MenuItemGrantParentId = id;
            foreach (var firstItem in (MenuItemList.Where(s => s.MenuItemParentID == id).ToList()))
            {
                ParentList.Add(new SelectListItem() { Text = firstItem.MenuName, Value = firstItem.MenuItemID.ToString() });
            }
        }
        public async Task MenuitemChildListlistFill(int? id)
        {
            MenuItemModal.MenuItemParentID = id;
            foreach (var secondItem in (MenuItemList.Where(s => s.MenuItemParentID == id).ToList()))
            {
                ChildList.Add(new SelectListItem() { Text = secondItem.MenuName, Value = secondItem.MenuItemID.ToString() });
            }
        }
        protected override async Task OnInitializedAsync()
        {
            //MenuItemList = await IuserServices.GetMenuItemList();
            //foreach (var item in MenuItemList.Where(s => s.MenuItemParentID == null).ToList())
            //{
            //    foreach (var firstItem in (MenuItemList.Where(s => s.MenuItemParentID == item.MenuItemID).ToList()))
            //    {
            //        GrantParentList.Add(new SelectListItem() { Text = firstItem.MenuName, Value = firstItem.MenuItemID.ToString() });
            //    }
            //}
        }
        protected override async Task OnParametersSetAsync()
        {

            if (IsVisible == true && MenuItemId != 0)
            {
                MenuItemModal = new();
                //foreach (var item in MenuItemList.Where(s => s.MenuItemParentID == null).ToList())
                //{
                //    foreach (var firstItem in (MenuItemList.Where(s => s.MenuItemParentID == item.MenuItemID).ToList()))
                //    {
                //        GrantParentList.Add(new SelectListItem() { Text = firstItem.MenuName, Value = firstItem.MenuItemID.ToString() });
                //        foreach (var secondItem in (MenuItemList.Where(s => s.MenuItemParentID == firstItem.MenuItemID).ToList()))
                //        {
                //            ParentList.Add(new SelectListItem() { Text = secondItem.MenuName, Value = secondItem.MenuItemID.ToString() });
                //            foreach (var Thrd in (MenuItemList.Where(s => s.MenuItemParentID == secondItem.MenuItemID).ToList()))
                //            {
                //                ChildList.Add(new SelectListItem() { Text = Thrd.MenuName, Value = Thrd.MenuItemID.ToString() });
                //            }
                //        }
                //    }
                //}
                MenuItemModalOrg = await IuserServices.NavMenuLinkByyId(MenuItemId);
                if (MenuItemModalOrg != null)
                {
                    //switch (MenuItemModalOrg.Level)
                    //{
                    //    case 1:
                    //        break;
                    //    case 2:
                    //        await MenuitemParentlistFill(MenuItemModalOrg.MenuItemParentID);
                    //        MenuItemModal.MenuItemGrantParentId = MenuItemModalOrg.MenuItemParentID;
                    //        break;
                    //    case 3:
                    //        await MenuitemParentlistFill(MenuItemModalOrg.MenuItems.MenuItems.MenuItemID);
                    //        await MenuitemChildListlistFill(MenuItemModalOrg.MenuItemParentID);
                    //        MenuItemModal.MenuItemGrantParentId = MenuItemModalOrg.MenuItems.MenuItems.MenuItemID;
                    //        MenuItemModal.MenuItemParentID = MenuItemModalOrg.MenuItemParentID;
                    //        break;
                    //    case 4:
                    //        MenuItemModal.MenuItemGrantParentId = MenuItemModalOrg.MenuItems.MenuItems.MenuItems.MenuItemID;
                    //        MenuItemModal.MenuItemParentID = MenuItemModalOrg.MenuItems.MenuItems.MenuItemID;
                    //        MenuItemModal.MenuItemChildId = MenuItemModalOrg.MenuItemParentID;
                    //        break;
                    //}
                    MenuItemModal.MenuItemParentID = MenuItemModalOrg.MenuItemParentID;
                    MenuItemModal.MenuName = MenuItemModalOrg.MenuName;
                    MenuItemModal.ActionLink = MenuItemModalOrg.ActionLink;
                    MenuItemModal.Icons = MenuItemModalOrg.Icons;
                    MenuItemModal.MenuItemID = MenuItemModalOrg.MenuItemID;
                    MenuItemModal.MenuItemType = MenuItemModalOrg.Type;
                }
            }
        }
        public Task CloseSideBar()
        {
            MenuItemModal = new();
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            MenuItemModal = new();
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public TostModel TostModelclass { get; set; } = new();
        public async Task SaveAddNewLinkData()
        {

            IsloaderShow = true;
            MenuItemModal.MenuItemParentID = MenuItemParentID;
            var registerResponse = await IuserServices.AddNavMenuLink(MenuItemModal);
            if (registerResponse.Message == "1" || registerResponse.Message == "2")
            {
                IsloaderShow = false;
                MenuItemModal = new();
                await OnAddSuccess.InvokeAsync(true);
                TostModelclass = registerResponse.Message.AlertSuccessMessage();

                //String[] agr = { };
                //Program.Main(agr);
            }
            else
            {
                IsloaderShow = false;
                TostModelclass = registerResponse.Message.AlertErrorMessage();
            }
        }
    }
}
