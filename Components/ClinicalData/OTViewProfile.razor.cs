using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Helpers.Enums;
using ArdantOffical.IService;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using SalesforceSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ArdantOffical.Components.ClinicalData
{
    public partial class OTViewProfile
    {
        [Inject]
        IUsersServices UsersServices { get; set; }
        [Parameter]
        public string SFUserId { get; set; }
        [Parameter]
        public Action RefreshMenuHeader { get; set; }
        public List<SelectListItem> lstGender = new();
        public List<SelectListItem> lstTitles = new();
        IEnumerable<MulticheckboxList> lstPerferences;
        IEnumerable<MulticheckboxList> lstStates;
        public List<AttachmentsVm> attachments = new List<AttachmentsVm>();
        public AttachmentsVm attachment = new AttachmentsVm();
        IEnumerable<string> lstSelectedIds = new string[] { };
        IEnumerable<string> lstSelectedStates = new string[] { };
        [Inject]
        IWebHostEnvironment Environment { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsloaderShow { get; set; }
        public TostModel TostModelclass { get; set; } = new();
        [Inject]
        AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        public OTVm OTVm = new OTVm();

        MulticheckboxList[] Preferences = new MulticheckboxList[]            {

              new MulticheckboxList {Name="NDIS" },
              new MulticheckboxList {Name="HCP" },
              new MulticheckboxList {Name="DVA" }
            };



        public IEnumerable<MulticheckboxList> GetPreferences()
        {

            return Preferences;
        }

        MulticheckboxList[] States = new MulticheckboxList[]            {

                new MulticheckboxList {Name="QLD" },
                new MulticheckboxList {Name="VIC" },
                new MulticheckboxList {Name= "NSW" },
                new MulticheckboxList { Name = "SA" },
                new MulticheckboxList { Name = "WA" },
                new MulticheckboxList { Name = "TAS" },
                new MulticheckboxList { Name = "ACT" },
                new MulticheckboxList { Name = "NT" }
            };



        public IEnumerable<MulticheckboxList> GetStates()
        {

            return States;
        }
        private void GetGender()
        {
            lstGender.Add(new SelectListItem() { Value = "Female", Text = "Female" });
            lstGender.Add(new SelectListItem() { Value = "Male", Text = "Male" });
            lstGender.Add(new SelectListItem() { Value = "Non-binary", Text = "Non-binary" });
            lstGender.Add(new SelectListItem() { Value = "Prefer not to say", Text = "Prefer not to say" });
            lstGender.Add(new SelectListItem() { Value = "Other", Text = "Other" });

        }
        private void GetTitles()
        {
            lstTitles.Add(new SelectListItem() { Value = "Miss", Text = "Miss" });
            lstTitles.Add(new SelectListItem() { Value = "Mr.", Text = "Mr." });
            lstTitles.Add(new SelectListItem() { Value = "Ms.", Text = "Ms." });
            lstTitles.Add(new SelectListItem() { Value = "Mrs.", Text = "Mrs." });
            lstTitles.Add(new SelectListItem() { Value = "Dr.", Text = "Dr." });

        }
        private DotNetObjectReference<OTViewProfile> dotNetObjectReference;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsloaderShow = true;
                dotNetObjectReference = DotNetObjectReference.Create(this);
                lstPerferences = GetPreferences();
                lstStates = GetStates();
                await LoadAttachments();
                GetGender();
                GetTitles();
                await LoadOTData();
                await LoadOTProfile();

                IsloaderShow = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format("ERROR : {0}", ex.Message);
            }

        }

        [Parameter]
        public EventCallback VisibilityHide { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        public async Task CloseSideBar()
        {
            await VisibilityHide.InvokeAsync();
        }

        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public async Task LoadOTData()
        {
            try
            {
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();

                var records = SFConnect.client.Query<OT>($"SELECT Id,Firstname__c,Lastname__c,Email__c, Phone__c, AHPRA_No__c, " +
                    $" IsEmployed__c, Gender__c,Bank_Account_BSB__c, Bank_Account_Number__c, Rights__c, DoB__c, Experience__c,  " +
                    $" Hours_per_week__c, HearAboutUs__c, Insurance__c, Medicare_Provider_No__c, NDIS_Registered__c, Open_to_Telehealth__c, Preference__c,State__c,  " +
                    $" Title__c, StartWorkingDate__c, Willing_to_travel_KM__c, ABN__c   FROM  OTs__c  WHERE  Id = '{SFUserId}'  LIMIT 1");
                var record = records.FirstOrDefault(a => a.Id == SFUserId);
                if (record != null)
                {
                    OTVm.Firstname = record.Firstname__c;
                    OTVm.Lastname = record.Lastname__c;
                    OTVm.Gender = record.Gender__c;
                    OTVm.Title = record.Title__c;
                    OTVm.Email = record.Email__c;
                    OTVm.Phone = record.Phone__c;
                    OTVm.AHPRANo = record.AHPRA_No__c;
                    OTVm.IsEmployed__c = record.IsEmployed__c;
                    OTVm.Bank_Account_BSB__c = record.Bank_Account_BSB__c;
                    OTVm.Bank_Account_Number__c = record.Bank_Account_Number__c;
                    OTVm.ABN = record.ABN__c;
                    OTVm.Right__c = record.Rights__c;
                    //OTVm.DoB__c = record.DoB__c;
                    OTVm.Experience__c = record.Experience__c;
                    OTVm.StartWorkingDate__c = record.StartWorkingDate__c;
                    OTVm.Willing_to_travel_KM__c = record.Willing_to_travel_KM__c;
                    OTVm.Open_to_Telehealth__c = record.Open_to_Telehealth__c;
                    OTVm.State__c = record.State__c;
                    OTVm.Hours_per_week__c = record.Hours_per_week__c;
                    OTVm.HearAboutUs__c = record.HearAboutUs__c;
                    OTVm.Insurance__c = record.Insurance__c;
                    OTVm.Medicare_Provider_No__c = record.Medicare_Provider_No__c;
                    OTVm.NDIS_Registered__c = record.NDIS_Registered__c;
                    OTVm.Preference__c = record.Preference__c;

                }
                if (OTVm.Preference__c != null)
                {
                    lstSelectedIds = OTVm.Preference__c.Split(',');
                }

                if (OTVm.State__c != null)
                {
                    lstSelectedStates = OTVm.State__c.Split(',');
                }


            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }
        public async Task LoadOTProfile()
        {
            attachments = await UsersServices.GetOT_ProfilePic(SFUserId);

        }
        public async Task UpdateOT_Profile()
        {

            await UsersServices.UpdateOT_Profile(attachments);
            RefreshMenuHeader?.Invoke();
        }


        public string fileUpload = "fileUploadId";
        public string signaturefile = "signaturefileId";
        private async Task BuisnessFileupdate(string id)
        {
            try
            {
                // DisableState = true;
                StateHasChanged();
                Thread.Sleep(200);

                //await js.InvokeAsync<List<string>>("MyFileUploadFunctionForOnBoarding", dotNetObjectReference, id);
                await js.InvokeAsync<List<string>>("MyFileUploadFunctionForOnBoarding", dotNetObjectReference, id);
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }
        }

        public async Task LoadAttachments()
        {
            // attachments = await AttachmentServices.GetAllAttachments(JobId);
        }

        public static List<string> MyFileArray = new List<string>();
        [JSInvokable]
        public void ChangeParaContentValueForOnBoarding(List<string> value)
        {
            // DepositorModel.Filenames = new List<string>();
            //attachments = new List<AttachmentsVm>();
           
            var attachment = new AttachmentsVm();
            MyFileArray = value;
            attachment.Path = value[0];
            attachment.Title = value[2];
            attachment.Folder = value[3];
            attachment.UserFileType = FileType;
            attachment.SalesforceID = SFUserId;
            attachments.Add(attachment);

            
        }



        #region File Upload Progress Bar

        private double progressPercentage = 0;
        private string progressBarDisplay = "none";
        public int FileId { get; set; }
        public string FileValidationMessage { get; set; }
        public int maxWidth;
        public int maxHeight;
        public long maxFileSizeBytes = 1024 * 1024;
        public UserFileType FileType { get; set; }
        public async Task OnInputFileChange(InputFileChangeEventArgs e, UserFileType userFileType)
        {
            try
            {
                var file = e.File;
                if (file != null)
                {
                    FileType = userFileType;

                    if (attachments.Any(a => a.SalesforceID == SFUserId && a.UserFileType == FileType))
                    {
                        var attachment = attachments.Where(a => a.SalesforceID == SFUserId && a.UserFileType == FileType).FirstOrDefault();
                        RemoveExistingFilePath(attachment);
                    }


                    var imageBytes = new byte[e.File.Size];
                    await e.File.OpenReadStream().ReadAsync(imageBytes);
                    using (var image = System.Drawing.Image.FromStream(new MemoryStream(imageBytes)))
                    {
                        maxWidth = image.Width;
                        maxHeight = image.Height;
                    }
                    if (maxWidth > 200 || maxHeight > 200)
                    {
                        FileValidationMessage = "The image dimensions exceed the specified limit.";
                        return;
                    }
                    // Show progress bar
                    FileValidationMessage = string.Empty;
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
                    if (FileType == UserFileType.Signature)
                    {
                        await BuisnessFileupdate(signaturefile);
                    }
                    else
                    {
                        await BuisnessFileupdate(fileUpload);
                    }

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
        public void RemoveExistingFilePath(AttachmentsVm attachment)
        {
            if (attachment.Folder != null && attachment.Path != null)
            {
                var existingFile = Path.Combine(Environment.WebRootPath, "Documents/" + attachment.Folder, attachment.Path);
                if (File.Exists(existingFile))
                {
                    File.Delete(existingFile);
                }
                attachments.Remove(attachments.Where(a => a.UserFileType == FileType).FirstOrDefault());
               
            }
        }
        public async Task SaveOTData()
        {
            IsloaderShow = true;
            try
            {
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                OTVm.Preference__c = string.Empty;
                foreach (var item in lstSelectedIds)
                {
                    OTVm.Preference__c = OTVm.Preference__c + item + ",";
                }

                OTVm.Preference__c = OTVm.Preference__c.TrimEnd(',');

                OTVm.State__c = string.Empty;
                foreach (var item in lstSelectedStates)
                {
                    OTVm.State__c = OTVm.State__c + item + ",";
                }

                OTVm.State__c = OTVm.State__c.TrimEnd(',');

                SFConnect.client.Update("OTs__c", Userinfo.SalesforceID, new
                {
                    Firstname__c = OTVm.Firstname,
                    Lastname__c = OTVm.Lastname,
                    Gender__c = OTVm.Gender,
                    Title__c = OTVm.Title,
                    Email__c = OTVm.Email,
                    Phone__c = OTVm.Phone,
                    AHPRA_No__c = OTVm.AHPRANo,
                    ABN__c = OTVm.ABN,
                    IsEmployed__c = OTVm.IsEmployed__c,
                    Bank_Account_BSB__c = OTVm.Bank_Account_BSB__c,
                    Bank_Account_Number__c = OTVm.Bank_Account_Number__c,
                    Rights__c = OTVm.Right__c,
                    //OTVm.DoB__c = record.DoB__c;
                    Experience__c = OTVm.Experience__c,
                    StartWorkingDate__c = OTVm.StartWorkingDate__c,
                    Willing_to_travel_KM__c = OTVm.Willing_to_travel_KM__c,
                    Open_to_Telehealth__c = OTVm.Open_to_Telehealth__c,
                    State__c = OTVm.State__c,
                    Hours_per_week__c = OTVm.Hours_per_week__c,
                    HearAboutUs__c = OTVm.HearAboutUs__c,
                    Insurance__c = OTVm.Insurance__c,
                    Medicare_Provider_No__c = OTVm.Medicare_Provider_No__c,
                    NDIS_Registered__c = OTVm.NDIS_Registered__c,
                    Preference__c = OTVm.Preference__c
                });
                await UpdateOT_Profile();
                responseHeader = "Success";
                responseBody = "Profile updated.";
                responseDialogVisibility = true;
                IsloaderShow = false;
                //await VisibilityHide.InvokeAsync();
                //  await OnAddSuccess.InvokeAsync(true);
            }
            catch (SalesforceException ex)
            {
                IsloaderShow = false;
                responseHeader = "ERROR";
                responseBody = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
                responseDialogVisibility = true;


            }
        }


        #endregion

        public class MulticheckboxList
        {
            public string Name { get; set; }
        }

    }
}
