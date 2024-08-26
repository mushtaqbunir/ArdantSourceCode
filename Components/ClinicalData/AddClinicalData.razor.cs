using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Data;
using ArdantOffical.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ArdantOffical.Data.ModelVm.ClinicalData;
using ArdantOffical.Models;
using ArdantOffical.Services;
using SalesforceSharp;
using System.Net;
using DocumentFormat.OpenXml.Wordprocessing;
using ArdantOffical.Data.ModelVm;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ArdantOffical.Helpers.Enums;
using Humanizer;

namespace ArdantOffical.Components.ClinicalData
{
    public partial class AddClinicalData
    {
      
        [Parameter]
        public bool IsVisible { get; set; }
        [Parameter]
        public string JobId { get; set; }

        [Parameter]
        public string JobType { get; set; }

        private int currentPage = 1;
        private int totalPageQuantity;
        private int totalCount = -1;
        public PaginationDTO paginationObj { get; set; } = new PaginationDTO();
        public NotesVMForTable ListOfRecord { get; set; }
        public List<NotesVM> Notes { get; set; }
        public NotesVM NotesModal = new NotesVM();
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
        public TostModel TostModelclass { get; set; } = new();
        [Inject]
        public FGCDbContext Context { get; set; }
        public string ActionName { get; set; } = "Save";


        public Task CloseSideBar()
        {
            NotesModal = new NotesVM();
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            NotesModal = new NotesVM();
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public async Task SaveData()
        {
            IsloaderShow = true;       
            ConnectSalesforce();
            // Call the create method to create the record
            try
            {
                if(ActionName=="Save")
                {
                    Exception registerResponse = null;
                    // create a record using an anonymous class and returns the ID
                    switch (JobType)
                    {
                        case "NDIS":
                            registerResponse = await IclinicalDataServices.AddNDISNote(NotesModal, JobId);
                            break;
                        case "HCP":
                            registerResponse = await IclinicalDataServices.AddHCPNote(NotesModal, JobId);
                            break;
                        case "DVA":
                            registerResponse = await IclinicalDataServices.AddDVANote(NotesModal, JobId);
                            break;
                    }
                  
                    if (registerResponse.Message == "1")
                    {                                        
                        TostModelclass.AlertMessageShow = true;
                        TostModelclass.AlertMessagebody = "Note Record Saved";
                        TostModelclass.Msgstyle = MessageColor.Success;
                        NotesModal=new NotesVM();                        
                    }
                    else
                    {                      
                       
                        TostModelclass.AlertMessageShow = true;
                        TostModelclass.AlertMessagebody = registerResponse.Message;
                        TostModelclass.Msgstyle = MessageColor.Error;

                    }
                } else if(ActionName=="Update")
                {
                    Exception registerResponse = null;
                    // create a record using an anonymous class and returns the ID
                    switch (JobType)
                    {
                        case "NDIS":
                            registerResponse = await IclinicalDataServices.UpdateNDISNote(NotesModal, Id);
                            break;
                        case "HCP":
                            registerResponse = await IclinicalDataServices.UpdateHCPNote(NotesModal, Id);
                            break;
                        case "DVA":
                            registerResponse = await IclinicalDataServices.UpdateDVANote(NotesModal, JobId);
                            break;
                    }
                    if (registerResponse.Message == "1")
                    {
                        TostModelclass.AlertMessageShow = true;
                        TostModelclass.AlertMessagebody = "Note Record Updated";
                        TostModelclass.Msgstyle = MessageColor.Success;
                        NotesModal = new NotesVM();
                    }
                    else
                    {
                        TostModelclass.AlertMessageShow = true;
                        TostModelclass.AlertMessagebody = registerResponse.Message;
                        TostModelclass.Msgstyle = MessageColor.Error;
                    }
                }               
                IsloaderShow = false;
               await LoadRecords();
            }
            catch (SalesforceException ex)
            {
                IsloaderShow = false;           
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = " Failed to save note: " +  ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;

            }

        }

        public bool ShowDetailsVisibility { get; set; } = false;
        public string MessageDetail { get; set; } = "";
        public string HeaderTitle { get; set; } = "";
        public async Task ShowMessage(string message, string caption)
        {
            MessageDetail = message;
            HeaderTitle = caption;
            OnShowMessageVisibilityChanged(true);
            StateHasChanged();
        }
        public void OnShowMessageVisibilityChanged(bool visibilityStatus)
        {
            ShowDetailsVisibility = visibilityStatus;
        }

        private  void ConnectSalesforce()
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
                IsloaderShow = false;
                responseHeader = "ERROR";
                responseBody = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
                responseDialogVisibility = true;

            }
        }




        protected override async Task OnInitializedAsync()
        {
             ConnectSalesforce();
            NotesModal.Title = "New Clinical Note [" + DateTime.Now.ToString("dd/MM/yyyy") + "]";
            await LoadRecords();
        }
        public string Id { get; set; }
        public async Task EditNotes(string id)
        {
            Id = id;
            switch (JobType)
            {
                case "NDIS":
                    NotesModal = await IclinicalDataServices.GetNDISNoteByID(Id);
                    break;
                case "HCP":
                    NotesModal = await IclinicalDataServices.GetHCPNoteByID(Id);
                    break;
                case "DVA":
                    NotesModal = await IclinicalDataServices.GetDVANoteByID(Id);
                    break;
            }
           
            ActionName = "Update";
        }
        async Task LoadRecords(int page = 1, int quantityPerPage = 200)
        {
            try
            {
                currentPage = page;
                paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
                switch(JobType)
                {
                    case "NDIS":
                        ListOfRecord = await IclinicalDataServices.GetNDISNotes(JobId);
                        break;
                    case "HCP":
                        ListOfRecord = await IclinicalDataServices.GetHCPNotes(JobId);
                        break;
                    case "DVA":
                        ListOfRecord = await IclinicalDataServices.GetDVANotes(JobId);
                        break;
                }
             
                if (ListOfRecord != null)
                {
                    Notes = ListOfRecord.Notes;                  
                }
            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
                throw;
            }

        }

        public bool DeleteConfirmationVisibility { get; set; }
        public void OnDeleteConfirmationVisibilityChangedModel(bool visibilityStatus)
        {
            DeleteConfirmationVisibility = visibilityStatus;

        }
        public bool IsDelete { get; set; } = false;
        async Task DeleteNote(string id)
        {
            try
            {
                DeleteConfirmationVisibility = true;
                Id = id;
            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
            }

        }
        public async void OnDeleteConfirmationSuccess(bool isAdded)
        {
            if (isAdded)
            {
                DeleteConfirmationVisibility = false;
                Exception registerResponse = null;
                switch (JobType)
                {
                    case "NDIS":
                        registerResponse = await IclinicalDataServices.DeleteNDISNote(Id);                       
                        break;
                    case "HCP":
                        registerResponse = await IclinicalDataServices.DeleteHCPNote(Id);                       
                        break;
                    case "DVA":
                        registerResponse = await IclinicalDataServices.DeleteDVANote(Id);                      
                        break;
                }
             
                if (registerResponse.Message == "1")
                {
                    TostModelclass.AlertMessageShow = true;
                    TostModelclass.AlertMessagebody = "Note Record Deleted";
                    TostModelclass.Msgstyle = MessageColor.Success;
                    await LoadRecords();
                }
                //StateHasChanged();
            }
        }


    }
}
