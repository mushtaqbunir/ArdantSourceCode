using ArdantOffical.Data.ModelVm.OT;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArdantOffical.Data;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Models;
using ArdantOffical.Services;
using SalesforceSharp;
using System.Linq;
using System;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Helpers.Enums;
using System.Runtime.CompilerServices;

namespace ArdantOffical.Components.Jobs_Details
{
    public partial class NDISDetail
    {
        [Inject]
        AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        [Parameter]
        public string JobId { get; set; }
        public string ErrorMessage { get; set; }

        ReferralFormNDISVm NDISVm = new ReferralFormNDISVm();
        public bool IsloaderShow { get; set; }
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public TostModel TostModelclass { get; set; } = new();
        //public bool IsSpinner { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }
    
        [Parameter]
        public EventCallback<bool> OnAddSuccess { get; set; }


        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public async Task CloseSideBar()
        {
            //await VisibilityHide.InvokeAsync();
            await OnVisibilityChanged.InvokeAsync(true);
        }      

        protected override async Task OnInitializedAsync()
        {
            IsloaderShow = true;
     
            await LoadNDISJobs();
            IsloaderShow = false;
        }
        public async Task LoadNDISJobs()
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

                var records = SFConnect.client.Query<NDIS>($"SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,Street_Address__c, Suburb__c, Postcode__c, State__c, P_Phone__c ,Language__c ,Medical_Background__c,Management_Type__c ,NDIS_Plan_Number__c," +
                    $" Allergies_or_Alerts__c,Name_of_Referrer__c, Position_Title__c,Organization__c, Coordinator_Email__c, Gender__c, Gender_Abbrev__c, " +
                    $" Coordinator_Phone__c,PrimaryContact_Name__c,Relationship_to_Client__c, PrimaryContactPhone__c, PrimaryContactEmail__c,PersonsRequiredAtVisit__c, " +
                    $" Reason_for_Referral__c,Name_of_Referrer_Secondary__c, Position_Title_Secondary__c,Organization_Secondary__c, Email_Secondary__c," +
                    $" Phone_Secondary__c, Inv_Name__c,Inv_Email__c,Inv_Phone__c,Inv_Address__c, Social_Information__c,Cultural_Requirements__c, Behavioural_Issues__c, " +
                    $" Pets__c,Manual_handling_issues__c, Onsite_Parking__c, Notes__c, Smoker__c, Other_risks_alerts__c, OT__c, Status__c , Job_Number__c          FROM  NDIS_Job__c  WHERE  Id = '{JobId}'  LIMIT 1");
                var record = records.FirstOrDefault(a => a.Id == JobId);
                if (record != null)
                {
                    NDISVm.Job_Number__c = record.Job_Number__c;
                    NDISVm.FirstName__c = record.P_Firstname__c;
                    NDISVm.SureName__c = record.P_Surname__c;
                    NDISVm.Title__c = record.Title__c;
                    NDISVm.DOBStr__c = record.Date_of_Birth__c.ToString();
                    //NDISVm.DOB__c = record.Date_of_Birth__c;
                    NDISVm.Street_Address__c = record.Street_Address__c;
                    NDISVm.Suburb__c = record.Suburb__c;
                    NDISVm.Postcode__c = record.Postcode__c;
                    NDISVm.State__c = record.State__c;
                    NDISVm.Phone__c = record.P_Phone__c;
                    NDISVm.Language__c = record.Language__c;
                    NDISVm.Gender = record.Gender__c;
                    NDISVm.MedicalBackground__c = record.Medical_Background__c;
                    NDISVm.NDISManagementType__c = record.Management_Type__c;
                    NDISVm.NDISPlanNumber__c = record.NDIS_Plan_Number__c;
                    NDISVm.AllergiesorAlerts__c = record.Allergies_or_Alerts__c;
                    NDISVm.NameofReferrer__c = record.Name_of_Referrer__c;
                    NDISVm.PositionTitle__c = record.Position_Title__c;
                    NDISVm.Organisation__c = record.Organization__c;
                    NDISVm.ReferrerEmail__c = record.Coordinator_Email__c;
                    NDISVm.ReferrerContactphone__c = record.Coordinator_Phone__c;
                    NDISVm.PrimaryContactFullName__c = record.PrimaryContact_Name__c;
                    NDISVm.PrimaryContactRelationshiptoClient__c = record.Relationship_to_Client__c;
                    NDISVm.PrimaryContactPhone__c = record.PrimaryContactPhone__c;
                    NDISVm.PrimaryContactEmail__c = record.PrimaryContactEmail__c;
                    NDISVm.PrimaryContactPersonVisit__c = record.PersonsRequiredAtVisit__c;
                    NDISVm.ReasonforReferral__c = record.Reason_for_Referral__c;
                    NDISVm.DifferentNameofReferrer__c = record.Name_of_Referrer_Secondary__c;
                    NDISVm.DifferentPositionTitle__c = record.Position_Title_Secondary__c;
                    NDISVm.DifferentOrganisation__c = record.Organization_Secondary__c;
                    NDISVm.DifferentReferrerEmail__c = record.Email_Secondary__c;
                    NDISVm.DifferentReferrerphone__c = record.Phone_Secondary__c;

                    NDISVm.InvoiceName__c = record.Inv_Name__c;
                    NDISVm.InvoiceAddress__c = record.Inv_Address__c;
                    NDISVm.InvoiceEmail__c = record.Inv_Email__c;
                    NDISVm.InvoicePhone__c = record.Inv_Phone__c;

                    NDISVm.SocialInformation__c = record.Social_Information__c;
                    NDISVm.CultureInformation__c = record.Cultural_Requirements__c;
                    NDISVm.BehaviouralIssues__c = record.Behavioural_Issues__c;
                    NDISVm.Pets__c = record.Pets__c;
                    NDISVm.EnviromentalIssues__c = record.Manual_handling_issues__c;
                    NDISVm.OnSitePark__c = record.Onsite_Parking__c;
                    NDISVm.Notes__c = record.Notes__c;
                    NDISVm.Smoker__c = record.Smoker__c;
                    NDISVm.OtherRisks__c = record.Other_risks_alerts__c;
                    NDISVm.Status__c = record.Status__c;
                    

                }

            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }

        public async Task ChooseJob()
        {
            IsSpinner= true;
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
                SFConnect.client.Update("NDIS_Job__c", JobId, new
                {
                    OT__c = Userinfo.SalesforceID,
                    Date_Allocated__c=Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK")),
                    Status__c = "Assigned"
                });
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = "This job is successfully added to your jobs list";
                TostModelclass.Msgstyle = MessageColor.Success;             
                IsloaderShow = false;
                IsSpinner = false;
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

        public bool IsSpinner { get; set; }
        public async Task Update()
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
                IsSpinner = true;
                await Task.Delay(10);
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                // create a record using an anonymous class and returns the ID
                string GenderShortName = string.Empty;
                switch (NDISVm.Gender)
                {
                    case "Female":
                        GenderShortName = "F";
                        break;
                    case "Male":
                        GenderShortName = "M";
                        break;
                    case "Other":
                        GenderShortName = "Other";
                        break;
                    case "Non-binary":
                        GenderShortName = "Non-binary";
                        break;
                    default:
                        GenderShortName = "NA";
                        break;
                }
            
                SFConnect.client.Update("NDIS_Job__c", JobId, new
                {
                    Title__c = NDISVm.Title__c,
                    P_Firstname__c = NDISVm.FirstName__c,
                    P_Surname__c = NDISVm.SureName__c,
                    Date_of_Birth__c = NDISVm.DOB__c,
                    Street_Address__c = NDISVm.Street_Address__c,
                    Suburb__c = NDISVm.Suburb__c,
                    Postcode__c = NDISVm.Postcode__c,
                    State__c = NDISVm.State__c,
                    P_Phone__c = NDISVm.Phone__c,
                    Language__c = NDISVm.Language__c,
                    Gender__c = NDISVm.Gender,
                    Gender_Abbrev__c = GenderShortName,
                    Medical_Background__c = NDISVm.MedicalBackground__c,
                    Allergies_or_Alerts__c = NDISVm.AllergiesorAlerts__c,
                    Management_Type__c = NDISVm.NDISManagementType__c,
                    NDIS_Plan_Number__c = NDISVm.NDISPlanNumber__c,

                    Name_of_Referrer__c = NDISVm.NameofReferrer__c,
                    Position_Title__c = NDISVm.PositionTitle__c,
                    Organization__c = NDISVm.Organisation__c,
                    Coordinator_Email__c = NDISVm.ReferrerEmail__c,
                    Coordinator_Phone__c = NDISVm.ReferrerContactphone__c,

                    PrimaryContact_Name__c = NDISVm.PrimaryContactFullName__c,
                    Relationship_to_Client__c = NDISVm.PrimaryContactRelationshiptoClient__c,
                    PrimaryContactPhone__c = NDISVm.PrimaryContactPhone__c,
                    PrimaryContactEmail__c = NDISVm.PrimaryContactEmail__c,
                    PersonsRequiredAtVisit__c = NDISVm.PrimaryContactPersonVisit__c,

                    Reason_for_Referral__c = NDISVm.ReasonforReferral__c,
                    Name_of_Referrer_Secondary__c = NDISVm.DifferentNameofReferrer__c,
                    Position_Title_Secondary__c = NDISVm.DifferentPositionTitle__c,
                    Organization_Secondary__c = NDISVm.DifferentOrganisation__c,
                    Email_Secondary__c = NDISVm.DifferentReferrerEmail__c,
                    Phone_Secondary__c = NDISVm.DifferentReferrerphone__c,

                    Inv_Name__c = NDISVm.InvoiceName__c,
                    Inv_Address__c = NDISVm.InvoiceAddress__c,
                    Inv_Email__c = NDISVm.InvoiceEmail__c,
                    Inv_Phone__c = NDISVm.InvoicePhone__c,

                    Social_Information__c = NDISVm.SocialInformation__c,
                    Cultural_Requirements__c = NDISVm.CultureInformation__c,
                    Behavioural_Issues__c = NDISVm.BehaviouralIssues__c,
                    Pets__c = NDISVm.Pets__c,
                    Manual_handling_issues__c = NDISVm.EnviromentalIssues__c,
                    Onsite_Parking__c = NDISVm.OnSitePark__c,
                    Notes__c = NDISVm.Notes__c,
                    Smoker__c = NDISVm.Smoker__c,
                    Other_risks_alerts__c = NDISVm.OtherRisks__c,
                    OT__c = Userinfo.SalesforceID,
                  
                });
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = "Job record updated.";
                TostModelclass.Msgstyle = MessageColor.Success;
                IsSpinner = false;
                //await VisibilityHide.InvokeAsync();

            }
            catch (SalesforceException ex)
            {
               
                responseHeader = "ERROR";
                responseBody = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
               


            }
        }

    }
}
