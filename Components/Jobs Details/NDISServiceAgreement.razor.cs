using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Invoices;
using ArdantOffical.Helpers.Enums;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using SalesforceSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Components.Jobs_Details
{
    public partial class NDISServiceAgreement
    {
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        private ElementReference canvasRef;
        public List<SelectListItem> lstJobs = new List<SelectListItem>();
        public string ErrorMessage { get; set; }
        public InvoicesVM InvoiceModal = new InvoicesVM();
        public TostModel TostModelclass { get; set; } = new();
        public bool IsloaderShow { get; set; } = false;
        private string signatureData;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
          
                await js.InvokeVoidAsync("initializeSignaturePad", canvasRef);
            
        }
        private async Task ClearSignature()
        {
            await js.InvokeVoidAsync("clearSignature", canvasRef);
        }

        private void ConnectSalesforce()
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
                //IsloaderShow = false;
                responseHeader = "ERROR";
                responseBody = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
                responseDialogVisibility = true;

            }
        }

        public Task SetJobID(string jobId)
        {
            try
            {
                InvoiceModal.JobID = jobId;
                GetParticipantDetails();

            }
            catch (Exception ex)
            {
                TostModelclass = ex.Message.AlertErrorMessage();
            }

            return Task.CompletedTask;
        }
        public void GetParticipantDetails()
        {
            ConnectSalesforce();
            try
            {
                var record = SFConnect.client.Query<NDIS>("SELECT Id,PrimaryContactEmail__c, Job_Number__c,P_Firstname__c,P_Surname__c,Street_Address__c,Suburb__c,Postcode__c, NDIS_Plan_Number__c  FROM NDIS_Job__c  WHERE Status__c= 'Assigned' AND Id='" + InvoiceModal.JobID + "' ");
                InvoiceModal.SentTo = record.FirstOrDefault().PrimaryContactEmail__c;
                InvoiceModal.Job_Number = record.FirstOrDefault().Job_Number__c;
                InvoiceModal.Customer_Name = record.FirstOrDefault().P_Firstname__c + ' ' + record.FirstOrDefault().P_Surname__c;
                InvoiceModal.Address = record.FirstOrDefault().Street_Address__c;
                InvoiceModal.Suburb = record.FirstOrDefault().Suburb__c;
                InvoiceModal.Postcode = record.FirstOrDefault().Postcode__c;
                InvoiceModal.NDISNumber = record.FirstOrDefault().NDIS_Plan_Number__c;
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }


        }

        public async Task LoadAssignedJobs()
        {

            //lstJobs = await IinvoiceServicesItems.GetAssignedNDISJobs();
            try
            {
                lstJobs = await IinvoiceServicesItems.GetAssignedJobs();
            }
            catch (Exception ex)
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = ex.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }

        }

    }
    

    }

