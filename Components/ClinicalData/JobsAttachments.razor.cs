using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Threading;
using System;
using ArdantOffical.Helpers;
using ArdantOffical.Data.ModelVm;
using NuGet.Packaging;
using ArdantOffical.Shared;
using System.Linq;
using ArdantOffical.IService;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components.Forms;
using ArdantOffical.Models;
using ArdantOffical.Services;
using ArdantOffical.Helpers.Enums;
using SalesforceSharp;

namespace ArdantOffical.Components.ClinicalData
{
    public partial class JobsAttachments
    {
        [Parameter]
        public bool IsVisible { get; set; }
        [Parameter]
        public string JobId { get; set; }
        public string UserAvailability = string.Empty;
        public string CssClass = string.Empty;
        public AddVM UserModal = new AddVM();
        public bool IsloaderShow { get; set; } = false;
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        [Parameter]
        public EventCallback OnVisibilityChanged { get; set; }
        [Parameter]
        public EventCallback<bool> OnAddSuccess { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }

        public List<SelectListItem> lstUserRoles = new();
        public List<SelectListItem> lstUserStatus = new();
        public TostModel TostModelclass { get; set; } = new();
        [Inject]
        public FGCDbContext Context { get; set; }
        [Inject]
        public IAttachmentServices AttachmentServices { get; set; }

        [Inject]
        IWebHostEnvironment Environment { get; set; }
        public Task CloseSideBar()
        {
            UserModal = new AddVM();
            return OnVisibilityChanged.InvokeAsync();
        }
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            UserModal = new AddVM();
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public async Task SaveData()
        {
            try
            {
                await BuisnessFileupdate(fileUpload);
                attachment.SalesforceID = JobId;
                // Save Attachment to Salesforce

                var filepath = Path.Combine(Environment.WebRootPath, "Documents/" + attachment.Folder, attachment.Path);
                byte[] data = File.ReadAllBytes(filepath);
                string fileID = SFConnect.client.Create("ContentVersion",
                       new
                       {
                           Title = attachment.Title,
                           PathOnClient = filepath,
                           VersionData = data,
                           Origin = "H",
                           FirstPublishLocationId = JobId


                       });

                await AttachmentServices.SaveAttachments(attachment);
                await LoadAttachments();
                attachment = new();
            }
            catch (SalesforceException ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }           

        }

        public async Task LoadAttachments()
        {
             attachments = await AttachmentServices.GetAllAttachments(JobId);
        }
        public Task UseEmailAsUsername()
        {
            UserModal.Username = UserModal.Email;
            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            dotNetObjectReference = DotNetObjectReference.Create(this);
            await LoadAttachments();
        }

        #region Document Upload
        public bool IsLoader { get; set; }
        private DotNetObjectReference<JobsAttachments> dotNetObjectReference;
        public List<AttachmentsVm> attachments = new List<AttachmentsVm>();
        public AttachmentsVm attachment = new AttachmentsVm();
        public DeleteConfirmationVm DeleteConfirmationVms { get; set; } = new DeleteConfirmationVm();
        

        public string fileUpload = "fileUploadId";
        private async Task BuisnessFileupdate(string id)
        {
            try
            {
                // DisableState = true;
                IsLoader = true;
                StateHasChanged();
                Thread.Sleep(200);              

                //await js.InvokeAsync<List<string>>("MyFileUploadFunctionForOnBoarding", dotNetObjectReference, id);
                await js.InvokeAsync<List<string>>("MyFileUploadFunctionForOnBoarding", dotNetObjectReference, fileUpload);
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }
        }
       

        public static List<string> MyFileArray = new List<string>();
        [JSInvokable]
        public void ChangeParaContentValueForOnBoarding(List<string> value)
        {
            // DepositorModel.Filenames = new List<string>();
            //attachments = new List<AttachmentsVm>();
            MyFileArray = value;
            attachment.Path = value[0];
            //attachment.Title = value[2];
            attachment.Folder = value[3];
            // attachments.Add(attachment); // Add the attachment to the list
           
            IsLoader = false;
            //PoliciesAndProcedures=0
            //if (MyType == 0)
            //{
            //    IsCompany1Show = false;
            //    AddRecord.BusinessId = ApplicationId;
            //    AddRecord.Type = FileTypes.PoliciesAndProcedures;
            //    AddRecord.FileNames.AddRange(value);
            //    AddSupportingDocumentsUpdate.Add(AddRecord);
            //    //LocalFileName.AddRange(value);
            //}

        }
        public bool DeleteConfirmationVisibility { get; set; }

        public void DeleteConfirmation(bool value)
        {
            if (value)
            {
                LocalDeleteFile(attachment);
            }
            else
            {
                DeleteConfirmationVisibility = false;
            }
        }
        public void LocalDeleteFile(AttachmentsVm attachmentVm)
        {
            DeleteConfirmationVms = new DeleteConfirmationVm();

            //IsDeleteConfirmation = true;
            //DeleteConfirmationVms.Name = filename;
            
            DeleteConfirmationVisibility = !DeleteConfirmationVisibility;
            if (DeleteConfirmationVisibility)
            {
                attachment = attachmentVm;
                return;
            }
            if (attachmentVm.ID > 0)
            {
                attachmentVm.Folder = attachments.Where(a => a.ID == attachmentVm.ID).Select(a=>a.Folder).FirstOrDefault();
                File_Delete(attachmentVm);
            }
            //DeleteConfirmationVms.Id = id;
            DeleteConfirmationVms.DeleteType = "0";
        }

        public async void File_Delete(AttachmentsVm attachmentsVm)
        {
            try
            {
                
                
                var path = Path.Combine(Environment.WebRootPath, "Documents/" + attachmentsVm.Folder,
                attachmentsVm.Path);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                if (attachmentsVm.ID > 0)
                {
                    await AttachmentServices.DeleteAttachments(attachmentsVm.ID);
                    await LoadAttachments();
                }
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }

            //attachments.Remove(attachments.Where(x => x.Path == attachmentsVm.Path).FirstOrDefault());

            StateHasChanged();
        }

        public bool SentConfirmationVisibility { get; set; }
        public string Title { get; set; }
        public string ConfirmationMessage { get; set; }
        public void DeleteFile(int DocumentId)
        {
            DeleteConfirmationVms = new DeleteConfirmationVm();
            DeleteConfirmationVms.Id = DocumentId;
            ConfirmationMessage = "Do you want to delete this attachment?";
            Title = "Delete File";
            DeleteConfirmationVms.DeleteType = "4";
        }

        public bool IsDeleteConfirmation { get; set; }
        public bool IsFile { get; set; }
        //public void DeleteConfirmation(bool value)
        //{
        //    IsDeleteConfirmation = true;
        //    IsFile = true;
        //    if (value)
        //    {

        //    }
        //    IsDeleteConfirmation = false;
        //}
        #endregion

        #region File Upload Progress Bar

        private double progressPercentage = 0;
        private string progressBarDisplay = "none";

        public async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;
                if (file != null)
                {
                   
                    // Show progress bar
                    progressBarDisplay = "block";
                    
                    StateHasChanged();

                    // Simulate file upload progress
                    for (int i = 0; i <= 100; i += 10)
                    {
                        UpdateProgress(i);
                        await Task.Delay(200); 
                    }

                    // Reset progress after upload is complete
                    UpdateProgress(0);
                    // Hide progress bar
                    progressBarDisplay = "none";
                    attachment.Title = Path.GetFileNameWithoutExtension(file.Name);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
            }
        }

        private void UpdateProgress(double percentage)
        {
            progressPercentage = percentage;
            StateHasChanged();
        }

        #endregion
    }
}
