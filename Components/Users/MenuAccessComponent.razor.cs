using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.IService;
using ArdantOffical.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace ArdantOffical.Components.Users
{
    public partial class MenuAccessComponent
    {
        [Inject]
        IUsersServices UsersServices { get; set; }
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
                MenuItemModel = await UsersServices.GetMenuItemAccessByUser(EditID);
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
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public bool IsloaderShow { get; set; } = false;
        public bool CloseFlag { get; set; } = false;
        public bool IsFileloaderShow { get; set; } = false;
        public async Task SubmitModel()
        {
            IsloaderShow = true;
            MenuItemCheckboxVM tt = MenuItemModel;
            //var registerResponse = await clientrepo.AddDepositor(MenuItemModel);
            //if (registerResponse.Message == "Add Depositor Data is added successfully." || registerResponse.Message == "Record updated successfully.")
            //{
            //    IsloaderShow = false;
            //    await OnAddSuccess.InvokeAsync(true);
            //    responseHeader = "Operation Successful";
            //    responseBody = registerResponse.Message;
            //    responseDialogVisibility = true;
            //}
            //else
            //{
            //    IsloaderShow = false;
            //    responseHeader = "Operation Failed";
            //    responseBody = registerResponse.Message;
            //    responseDialogVisibility = true;
            //}
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

        }
        private List<IBrowserFile> loadedFiles = new();
        private IEnumerable<IFormFile> filesss;
        IReadOnlyList<IBrowserFile> selectedFiles;
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
                        string[] FileExtension = DeleteConfirmationVms.Name.Split(',');
                        string[] split = DeleteConfirmationVms.Name.Split("/");
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

        async Task ToggleApprovedAsync()
        {

        }

        MenuItemFormModel AddModel = new();
        private async Task CheckChanged(ChangeEventArgs ev, string field)
        {
            var BoolValue = (System.Boolean)ev.Value;
            AddModel.MenuItemName = field;
            AddModel.UserId = EditID;
            AddModel.IsDelete = BoolValue;


            Exception registerResponse = await UsersServices.AddMenuAccess(AddModel);
            if (registerResponse.Message == "1" || registerResponse.Message == "2")
            {
                IsloaderShow = false;
                //await OnAddSuccess.InvokeAsync(true);
                responseHeader = "Operation Successful";
                responseBody = registerResponse.Message;
                responseDialogVisibility = true;
            }
            else
            {
                IsloaderShow = false;
                responseHeader = "Operation Failed";
                responseBody = registerResponse.Message;
                responseDialogVisibility = true;
            }

        }
    }
}
