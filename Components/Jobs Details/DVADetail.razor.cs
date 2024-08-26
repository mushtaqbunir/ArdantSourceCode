using ArdantOffical.Data.ModelVm.Dashboard;
using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components;
using SalesforceSharp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using ArdantOffical.Helpers.Extensions;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http.Headers;
using System.Runtime.CompilerServices;

namespace ArdantOffical.Components.Jobs_Details
{
    public partial class DVADetail
    {
        [Inject]
        AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        [Parameter]
        public string JobId { get; set; }

        ReferralFormDVAVm DVAVm = new ReferralFormDVAVm();
        public bool IsloaderShow { get; set; }
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }     
        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        [Parameter]
        public EventCallback<bool> OnAddSuccess { get; set; }
        [Parameter]
        public string ErrorMessage { get; set; }

        [Parameter]
        public EventCallback VisibilityHide { get; set; }
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public async Task CloseSideBar()
        {
           await VisibilityHide.InvokeAsync();
        }
        protected override async Task OnInitializedAsync()
        {
            IsloaderShow = true;
           await LoadDVAJobs();
            IsloaderShow = false;
        }
        public async Task LoadDVAJobs()
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
               
                var records = SFConnect.client.Query<DVA>($"SELECT Id, Condition_to_be_treated__c,Status__c, Given_Name__c, Email__c, Phone__c,CreatedDate, " +
                    $"Refferal__c,Surname__c,Dob__c,Address__c,Postcode__c,Mobile__c,Card_Type__c,Accepted_Disabilities__c,ReferralTo_Name__c," +
                    $"ReferralTo_Address__c,ReferralTo_Postcode__c,ReferralTo_Email__c,ReferralTo_Phone__c,ReferralTo_Mobile__c," +
                    $"Is_RACF_Patient__c,Class_of_care__c,Date_funding_began__c,Clinical_Details__c,Period_of_Referral__c,Other_treating_health_providers__c," +
                    $"Provider_Name__c,Provider_Number__c,Practice_Name__c,Practice_Address__c,Practice_Postcode__c,Practice_Email__c,Practice_Phone__c," +
                    $"Fax_Number__c, Job_Number__c   FROM DVA_Jobs__c  WHERE  Id = '{JobId}'  LIMIT 1");
                var record = records.FirstOrDefault(a=>a.Id==JobId);
                if (record != null)
                {
                    DVAVm.Job_Number__c = record.Job_Number__c;
                    DVAVm.ReferralType = record.Refferal__c;
                    DVAVm.PatientSurName = record.Surname__c;
                    DVAVm.PatientGivenName = record.Given_Name__c;
                    DVAVm.PateintDOB = record.Dob__c;
                    DVAVm.PatientAddress = record.Address__c;
                    DVAVm.PatientPostCode = record.Postcode__c;
                    DVAVm.PatientEmailAddress = record.Email__c;
                    DVAVm.PatientDVAFormNumber = record.DVA_FileNo__c;
                    DVAVm.PatientCardType = record.Card_Type__c;
                    DVAVm.PatientAcceptedDisabilities = record.Accepted_Disabilities__c;
                    DVAVm.ReferraltoName = record.ReferralTo_Name__c;
                    DVAVm.ReferraltoAddress = record.ReferralTo_Address__c;
                    DVAVm.ReferraltoPostCode = record.ReferralTo_Postcode__c;
                    DVAVm.ReferraltoEmailAddress = record.ReferralTo_Email__c;
                    DVAVm.ReferraltoPhoneNumber = record.ReferralTo_Phone__c;
                    DVAVm.ReferraltoMobileNumber = record.ReferralTo_Mobile__c;
                    //DVAVm.ReferraltoProviderNumber = record.ReferralTo;
                    DVAVm.ConditionTreated = record.Condition_to_be_treated__c;
                    DVAVm.IsPatientResidentialAged = bool.TryParse(record.Is_RACF_Patient__c, out bool result) ? result : false;
                    DVAVm.CarePatientClass = record.Class_of_care__c;
                    DVAVm.CarePatientDateFundingBegan = record.Date_funding_began__c;
                    DVAVm.ClinicalDetailsofCondition = record.Clinical_Details__c;
                    DVAVm.PeriodofReferral = record.Period_of_Referral__c;
                    DVAVm.OtherTreatingHealthProviders = record.Other_treating_health_providers__c;
                    DVAVm.ProviderName = record.Provider_Name__c;
                    DVAVm.ProviderNumber = record.Provider_Number__c;
                    DVAVm.ProviderPracticeName = record.Practice_Name__c;
                    DVAVm.ProviderPracticeAddress = record.Practice_Address__c;
                    DVAVm.ProviderName = record.Provider_Name__c;
                    DVAVm.ProviderPostCode = record.Practice_Postcode__c;
                    DVAVm.ProviderEmailAddress = record.Practice_Email__c;
                    DVAVm.ProviderPhoneNumber = record.Practice_Email__c;
                    DVAVm.ProviderFaxNumber = record.Fax_Number__c;
                    DVAVm.Status__c = record.Status__c;
                }
               
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }
        public bool IsSpinner { get; set; }
        public async Task ChooseJob()
        {
            IsSpinner = true;
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
                SFConnect.client.Update("DVA_Jobs__c", JobId, new
                {  
                    OT__c = Userinfo.SalesforceID,
                    Status__c = "Assigned",
                    Date_Allocated__c = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK"))
                });

                responseHeader = "Success";
                responseBody = "This job is successfully added to your jobs list";
                responseDialogVisibility = true;
                IsloaderShow = false;
                IsSpinner= false; 
                await VisibilityHide.InvokeAsync();
                await OnAddSuccess.InvokeAsync(true);

            }
            catch (SalesforceException ex)
            {
                IsloaderShow = false;
                responseHeader = "ERROR";
                responseBody = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
                responseDialogVisibility = true;


            }
        }
    }
}
