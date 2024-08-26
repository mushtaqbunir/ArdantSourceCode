using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Korzh.EasyQuery.DataModel;

namespace ArdantOffical.Components.Users
{
 
    public partial class MenuAccess
    {
        [Inject]
        public IUsersServices UsersServices { get; set; }
        [Parameter]
        public int EditID { get; set; }//user id
        [Parameter]
        public bool Visible { get; set; }
        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        [Parameter]
        public EventCallback<bool> OnAddSuccess { get; set; }
        [CascadingParameter]
        public Error? Error { get; set; }

        public bool DisableState { get; set; } = false;
        public int count = 0;
        protected override async Task OnParametersSetAsync()
        {
            if (EditID > 0 && Visible == true)
            {
                AddModel = new();
                // Remove the Delete option permission for the Admin user. As the admin by default have the delete permission
                List<int> DeleteMenuID = new List<int>() { 2167, 2168, 2169 };
                ListOfMenuItem = await UsersServices.GetMenuItemList();
                string UserRoleID = UsersServices.GetUserRole(EditID);
                if(UserRoleID=="1") // Admin Role ID is 1
                {
                    ListOfMenuItem = ListOfMenuItem.Where(x => !DeleteMenuID.Contains(x.MenuItemID)).ToList();
                }                
                ListOfUserMenuItem = await UsersServices.GetUserMenuItemList(EditID);
                ListOfRemoveAllCheckedAndUncheckedModel = new();

                // MenuItemModel = await IuserServices.GetMenuItemAccessByUser(EditID);
            }
            else
            {
                MenuItemModel = new();
            }
        }
        private ElementReference firstInput;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //await js.InvokeAsync("ShowModal", "AddDepositorModal");
            //  await   js.InvokeVoidAsync("ShowModal", "AddDepositorModal");
        }
        public string rootFolder;
        [Inject]
        public IWebHostEnvironment Environment { get; set; }
        public MenuItemCheckboxVM MenuItemModel = new();
        public List<MenuItem> ListOfMenuItem = new();
        public string Showplus { get; set; } = "";
        public List<RemoveAllCheckedAndUncheckedModel> ListOfRemoveAllCheckedAndUncheckedModel = new();

        public List<ListOfMenuItemFormModel> ListOfUserMenuItem = new();
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public bool IsloaderShow { get; set; } = false;
        public bool CloseFlag { get; set; } = false;
        public bool IsFileloaderShow { get; set; } = false;
        public TostModel TostModelclass { get; set; } = new();
        public async Task SubmitModel()
        {
            IsloaderShow = true;
            AddModel.UserId = EditID;
            AddModel.ListOfMenuItems = ListOfUserMenuItem.DistinctBy(x => x.MenuName).ToList();
            var registerResponse = await UsersServices.AddMenuAccess(AddModel);
            if (registerResponse.Message == "1" || registerResponse.Message == "2")
            {

                IsloaderShow = false;
                TostModelclass = registerResponse.Message.AlertSuccessMessage();
                await OnVisibilityChanged.InvokeAsync(false);
                //responseHeader = "Operation Successful";
                //responseBody = registerResponse.Message;
                //responseDialogVisibility = true;
            }
            else
            {
                IsloaderShow = false;
                TostModelclass = registerResponse.Message.AlertErrorMessage();
                //responseHeader = "Operation Failed";
                //responseBody = registerResponse.Message;
                //responseDialogVisibility = true;
            }
        }
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            MenuItemModel = new();
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public Task CloseSideBar()
        {
            MenuItemModel = new();

            return OnVisibilityChanged.InvokeAsync(false);
        }
        protected override async Task OnInitializedAsync()
        {
            rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/UploadImages");
            AddModel = new();
        }

        private long maxFileSize = 1024 * 1024 * 250;
        private int maxAllowedFiles = 20;
        private bool isLoading;
        private string displayImage;
        public string Errors { get; set; }
        public bool IsError { get; set; } = false;




        public DeleteConfirmationVm DeleteConfirmationVms { get; set; } = new DeleteConfirmationVm();
        public bool DeleteConfirmationVisibility { get; set; }
        public void OnDeleteConfirmationVisibilityChangedModel(bool visibilityStatus)
        {
            //IsInside = true;
            DeleteConfirmationVisibility = visibilityStatus;
        }
        public bool IsDelete { get; set; } = false;
        public async void OnDeleteConfirmationSuccess(bool isAdded)
        {

            //IsInside = true;
            if (isAdded)
            {
                if (DeleteConfirmationVms.DeleteType == "DeleteFile")
                {
                    DeleteConfirmationVisibility = false;
                    if (DeleteConfirmationVms.Name != null && DeleteConfirmationVms.Name != "")
                    {
                        var FileExtension = DeleteConfirmationVms.Name.Split(',');
                        var split = DeleteConfirmationVms.Name.Split("/");
                        //if (File.Exists(Path.Combine(rootFolder, split[2])))
                        if (File.Exists(Path.Combine(rootFolder, FileExtension[0])))
                        {
                            // If file found, delete it    
                            File.Delete(Path.Combine(rootFolder, FileExtension[0]));
                            //  File.Delete(Path.Combine(rootFolder, split[2]));
                        }
                        //LocalFileName.Remove(DeleteConfirmationVms.Name);
                        //DepositorModel.Filenames.Remove(FileExtension[0]);
                    }
                }
                // StateHasChanged();
            }
        }


        [Inject]
        public FGCDbContext context { get; set; }
        //List<string> Menulist = new();
        MenuItemFormModel AddModel = new();
        List<ListOfMenuItemFormModel> ListOfMenuItems = new();


        private async Task ALLCheckChanged(ChangeEventArgs ev, int? MenuItemParentID, int AllCheckedParentID, string field)
        {
            ListOfMenuItemFormModel Menulist = new();
            var BoolValue = (Boolean)ev.Value;
            AddModel.MenuItemName = field;
            AddModel.UserId = EditID;
            AddModel.MenuItemParentID = 0;
            AddModel.IsDelete = BoolValue;

            Menulist.MenuItemParentID = 0;
            Menulist.MenuName = field;
            var MenuName = "";
            var Fields = field.Split("-");
            if (Fields.Length > 1)
            {
                MenuName = Fields[1];
            }
            if (BoolValue == true)
            {
                ListOfUserMenuItem.Add(Menulist);
                var GetMenuItemParent = ListOfMenuItem.Where(x => x.MenuName == MenuName).ToList();
                foreach (var firstItem in GetMenuItemParent)
                {
                    if (!ListOfUserMenuItem.Any(x => x.MenuName == firstItem.MenuName))
                        ListOfUserMenuItem.Add(new ListOfMenuItemFormModel() { MenuItemParentID = AllCheckedParentID, MenuName = firstItem.MenuName });
                    if (firstItem.MenuItemChild != null)
                        foreach (var secondItem in firstItem.MenuItemChild)
                        {
                            if (!ListOfUserMenuItem.Any(x => x.MenuName == secondItem.MenuName))
                                ListOfUserMenuItem.Add(new ListOfMenuItemFormModel() { MenuItemParentID = AllCheckedParentID, MenuName = secondItem.MenuName });
                            if (secondItem.MenuItemChild != null)
                                foreach (var ThirdItem in secondItem.MenuItemChild)
                                {
                                    if (!ListOfUserMenuItem.Any(x => x.MenuName == ThirdItem.MenuName))
                                        ListOfUserMenuItem.Add(new ListOfMenuItemFormModel() { MenuItemParentID = AllCheckedParentID, MenuName = ThirdItem.MenuName });
                                    if (ThirdItem.MenuItemChild != null)
                                        foreach (var FouthItem in ThirdItem.MenuItemChild)
                                        {
                                            if (!ListOfUserMenuItem.Any(x => x.MenuName == FouthItem.MenuName))
                                                ListOfUserMenuItem.Add(new ListOfMenuItemFormModel() { MenuItemParentID = AllCheckedParentID, MenuName = FouthItem.MenuName });
                                        }
                                }
                        }
                }

            }
            else
            {
                await GetMenuItemParentUnChecked(MenuName, AllCheckedParentID);//uncheck 
                ListOfUserMenuItem.RemoveAll(x => x.MenuName == Menulist.MenuName);
            }
        }

        private async Task CheckChanged(ChangeEventArgs ev, int? MenuItemParentID, int? AllCheckedParentID, string field,int MenuItemId)
        {
            ListOfMenuItemFormModel Menulist = new();
            var BoolValue = (Boolean)ev.Value;
            AddModel.MenuItemName = field;
            AddModel.UserId = EditID;
            AddModel.MenuItemParentID = AllCheckedParentID;
            AddModel.IsDelete = BoolValue;
            Menulist.MenuName = field;
            Menulist.MenuItemParentID = AllCheckedParentID;
            #region Onyl for Country Rating and All Feathers
            //local
            //2187=RM Permissions
            //2188=All Features
            //2189=Country Ratings
            //live
            //2164=RM Permissions
            //2166=All Features
            //2165=Country Ratings
            if (MenuItemId== 2164 && BoolValue)
            {
                field = "All Features";
                AddModel.MenuItemName = field;
                Menulist.MenuName = field;
                if (!ListOfUserMenuItem.Any(x => x.MenuName == field))
                    ListOfUserMenuItem.Add(Menulist);
                ListOfUserMenuItem.Remove(ListOfUserMenuItem.Where(s => s.MenuName == "Country Ratings").FirstOrDefault());
                StateHasChanged();
                return;
            }
            else if (MenuItemId== 2165 || MenuItemId== 2166)
            {
                if(MenuItemId== 2165 && BoolValue)
                {
                    if (!ListOfUserMenuItem.Any(x => x.MenuName == field))
                        ListOfUserMenuItem.Add(Menulist);
                    ListOfUserMenuItem.Remove(ListOfUserMenuItem.Where(s => s.MenuName == "All Features").FirstOrDefault());
                }
                else if(MenuItemId == 2165 && !BoolValue)
                {
                    field = "All Features";
                    AddModel.MenuItemName = field;
                    Menulist.MenuName = field;
                    if (!ListOfUserMenuItem.Any(x => x.MenuName == field))
                        ListOfUserMenuItem.Add(Menulist);
                    ListOfUserMenuItem.Remove(ListOfUserMenuItem.Where(s => s.MenuName == "Country Ratings").FirstOrDefault());
                }
                else if (MenuItemId == 2166 && BoolValue)
                {
                    if (!ListOfUserMenuItem.Any(x => x.MenuName == field))
                        ListOfUserMenuItem.Add(Menulist);
                    ListOfUserMenuItem.Remove(ListOfUserMenuItem.Where(s => s.MenuName == "Country Ratings").FirstOrDefault());
                }
                else if (MenuItemId == 2166 && !BoolValue)
                {
                    field = "Country Ratings";
                    AddModel.MenuItemName = field;
                    Menulist.MenuName = field;
                    if (!ListOfUserMenuItem.Any(x => x.MenuName == field))
                        ListOfUserMenuItem.Add(Menulist);
                    ListOfUserMenuItem.Remove(ListOfUserMenuItem.Where(s => s.MenuName == "All Features").FirstOrDefault());
                }
                StateHasChanged();
                return;
            }
            #endregion
            if (BoolValue == true)
            {
                if (!ListOfUserMenuItem.Any(x => x.MenuName == field))
                    ListOfUserMenuItem.Add(Menulist);
                await GetMenuItemParentChecked(MenuItemParentID, AllCheckedParentID);//checked
            }
            else
            {
                await GetMenuItemParentUnChecked(field, AllCheckedParentID);//uncheck 
                ListOfUserMenuItem.RemoveAll(x => x.MenuName == Menulist.MenuName);
            }
            StateHasChanged();
        }
        public async Task GetMenuItemParentChecked(int? MenuItemParentID, int? AllCheckedParentID)
        {
            var tt = ListOfRemoveAllCheckedAndUncheckedModel;
            var GetMenuItemParentID = await context.MenuItems.Include(x => x.MenuItems).Include(v => v.MenuItemChild)
                                             .Where(x => x.MenuItemParentID == MenuItemParentID).FirstOrDefaultAsync();// first step
            if (GetMenuItemParentID.MenuItems != null)
                if (!ListOfUserMenuItem.Any(x => x.MenuName == GetMenuItemParentID.MenuItems.MenuName))
                    if (GetMenuItemParentID.MenuItems.MenuItemParentID != null)
                        ListOfUserMenuItem.Add(new ListOfMenuItemFormModel() { MenuItemParentID = AllCheckedParentID, MenuName = GetMenuItemParentID.MenuItems.MenuName });//first Parent
            if (GetMenuItemParentID.MenuItems.MenuItemParentID != null)
            {
                var secondParent = await context.MenuItems.Where(x => x.MenuItemID == GetMenuItemParentID.MenuItems.MenuItemParentID).FirstOrDefaultAsync(); //second step
                if (secondParent != null)
                {
                    if (secondParent.MenuItemParentID != null)
                        if (!ListOfUserMenuItem.Any(x => x.MenuName == secondParent.MenuName))
                            ListOfUserMenuItem.Add(new ListOfMenuItemFormModel() { MenuItemParentID = AllCheckedParentID, MenuName = secondParent.MenuName });//second Parent
                    if (secondParent.MenuItemParentID != null)
                    {
                        var ThirdItemParent = await context.MenuItems.Where(x => x.MenuItemID == secondParent.MenuItemParentID).FirstOrDefaultAsync();//Third step
                        if (ThirdItemParent != null)
                            if (ThirdItemParent.MenuItemParentID != null)
                                if (!ListOfUserMenuItem.Any(x => x.MenuName == ThirdItemParent.MenuName))
                                    ListOfUserMenuItem.Add(new ListOfMenuItemFormModel() { MenuItemParentID = AllCheckedParentID, MenuName = ThirdItemParent.MenuName });//Third Parent
                    }
                }
            }
            await AllCheckUnCheckMenuItemParentChecked(AllCheckedParentID);
        }
        public async Task GetMenuItemParentUnChecked(string field, int? AllCheckedParentID)
        {
            var GetMenuItemParent = ListOfMenuItem.Where(x => x.MenuName == field).ToList();

            foreach (var firstItem in GetMenuItemParent)
            {
                if (ListOfUserMenuItem.Any(x => x.MenuName == firstItem.MenuName))
                    ListOfUserMenuItem.RemoveAll(x => x.MenuName == firstItem.MenuName);
                if (firstItem.MenuItemChild != null)
                    foreach (var secondItem in firstItem.MenuItemChild)
                    {
                        if (ListOfUserMenuItem.Any(x => x.MenuName == secondItem.MenuName))
                            ListOfUserMenuItem.RemoveAll(x => x.MenuName == secondItem.MenuName);
                        if (secondItem.MenuItemChild != null)
                            foreach (var ThirdItem in secondItem.MenuItemChild)
                            {
                                if (ListOfUserMenuItem.Any(x => x.MenuName == ThirdItem.MenuName))
                                    ListOfUserMenuItem.RemoveAll(x => x.MenuName == ThirdItem.MenuName);
                                if (ThirdItem.MenuItemChild != null)
                                    foreach (var FouthItem in ThirdItem.MenuItemChild)
                                    {
                                        if (ListOfUserMenuItem.Any(x => x.MenuName == FouthItem.MenuName))
                                            ListOfUserMenuItem.RemoveAll(x => x.MenuName == FouthItem.MenuName);
                                    }
                            }
                    }
            }
            await AllCheckUnCheckMenuItemParentChecked(AllCheckedParentID);

        }

        public async Task AllCheckUnCheckMenuItemParentChecked(int? AllCheckedParentID)
        {

            var RemovedMenuName = "";
            int ExisingRecordCounter = 0;
            var GetMenuItemParent = ListOfMenuItem.Where(x => x.MenuItemID == AllCheckedParentID.Value).FirstOrDefault();
            ExisingRecordCounter = ListOfUserMenuItem.Where(x => x.MenuItemParentID == AllCheckedParentID).Count();
            var CheckedTotalCount = ListOfRemoveAllCheckedAndUncheckedModel.Where(x => x.ParentID == AllCheckedParentID).FirstOrDefault();
            if (CheckedTotalCount != null)
            {
                if (GetMenuItemParent != null)
                {
                    RemovedMenuName = "All-" + GetMenuItemParent.MenuName;
                }
                if (ExisingRecordCounter != CheckedTotalCount.Counter)
                {

                    ListOfUserMenuItem.RemoveAll(x => x.MenuName == RemovedMenuName);
                }
                else
                {

                    if (!ListOfUserMenuItem.Any(x => x.MenuName == RemovedMenuName))
                        ListOfUserMenuItem.Add(new ListOfMenuItemFormModel() { MenuItemParentID = 0, MenuName = RemovedMenuName });//second Parent
                }
            }


        }


    }
}
