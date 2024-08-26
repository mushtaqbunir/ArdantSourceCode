using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Data;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using SalesforceSharp;
using System.Threading.Tasks;
using System.Linq;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Data.ModelVm.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;

namespace ArdantOffical.Components.Jobs_Details
{
    public partial class HCPDetail
    {
        [Inject]
        AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        [Parameter]
        public string JobId { get; set; }
        public string ErrorMessage { get; set; }

        ReferralFormHCPVm HCPVm = new ReferralFormHCPVm();
        public bool IsloaderShow { get; set; }
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public List<SelectListItem> FundingTypes = new List<SelectListItem>();
        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        [Parameter]
        public EventCallback VisibilityHide { get; set; }
        [Parameter]
        public EventCallback<bool> OnAddSuccess { get; set; }

        public bool IsSpinner { get; set; }
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }
        public async Task CloseSideBar()
        {
            await VisibilityHide.InvokeAsync();
        }

        public void FundingTypesList()
        {
            FundingTypes.Add(new SelectListItem() { Value = "Home Care Package", Text = "Home Care Package" });
            FundingTypes.Add(new SelectListItem() { Value = "CHSP", Text = "CHSP" });
            FundingTypes.Add(new SelectListItem() { Value = "Transition Care Package", Text = "Transition Care Package" });
            FundingTypes.Add(new SelectListItem() { Value = "Privately funded", Text = "Privately funded" });
            FundingTypes.Add(new SelectListItem() { Value = "Other", Text = "Other" });
        }
        protected override async Task OnInitializedAsync()
        {
            IsloaderShow = true;
            FundingTypesList();
            await LoadHCPJobs();
            IsloaderShow = false;
        }
        public async Task LoadHCPJobs()
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

                var records = SFConnect.client.Query<HCP>($"SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,Street_Address__c, Suburb__c, Postcode__c, State__c, P_Phone__c ,Language__c ,Medical_Background__c,Funding_Type__c ,Funding_Sub_Type__c," +
                    $" Allergies_or_Alerts__c,Name_of_Referrer__c, Position_Title__c,Organization__c, Coordinator_Email__c, Gender__c, Gender_Abbrev__c," +
                    $" Coordinator_Phone__c,PrimaryContact_Name__c,Relationship_to_Client__c, PrimaryContactPhone__c,PersonsRequiredAtVisit__c, " +
                    $" Reason_for_Referral__c,Name_of_Referrer_Secondary__c, Position_Title_Secondary__c,Organization_Secondary__c, Email_Secondary__c," +
                    $" Phone_Secondary__c, Inv_Name__c,Inv_Email__c,Inv_Phone__c,Inv_Address__c, Social_Information__c,Cultural_Requirements__c, Behavioural_Issues__c, " +
                    $" Pets__c,Manual_handling_issues__c, Onsite_Parking__c, Notes__c, Smoker__c, Other_risks_alerts__c, OT__c, Status__c, Job_Number__c          FROM  HCP_Job__c  WHERE  Id = '{JobId}'  LIMIT 1");
                var record = records.FirstOrDefault(a => a.Id == JobId);
                if (record != null)
                {
                    HCPVm.Job_Number__c = record.Job_Number__c;
                    HCPVm.FirstName__c = record.P_Firstname__c;
                    HCPVm.SureName__c = record.P_Surname__c;
                    HCPVm.Title__c = record.Title__c;
                    HCPVm.DOB__c = record.Date_of_Birth__c;
                    HCPVm.Street_Address__c = record.Street_Address__c;
                    HCPVm.Suburb__c = record.Suburb__c;
                    HCPVm.Postcode__c = record.Postcode__c;
                    HCPVm.State__c = record.State__c;
                    HCPVm.Phone__c = record.P_Phone__c;
                    HCPVm.Language__c = record.Language__c;
                    HCPVm.Gender = record.Gender__c;
                    HCPVm.MedicalBackground__c = record.Medical_Background__c;
                    HCPVm.FundingTypes__c = record.Funding_Type__c;
                    HCPVm.HomeCarePackageLevel__C = record.Funding_Sub_Type__c;
                    HCPVm.AllergiesorAlerts__c = record.Allergies_or_Alerts__c;
                    HCPVm.NameofReferrer__c = record.Name_of_Referrer__c;
                    HCPVm.PositionTitle__c = record.Position_Title__c;
                    HCPVm.Organisation__c = record.Organization__c;
                    HCPVm.ReferrerEmail__c = record.Coordinator_Email__c;
                    HCPVm.ReferrerContactphone__c = record.Coordinator_Phone__c;
                    HCPVm.PrimaryContactFullName__c = record.PrimaryContact_Name__c;
                    HCPVm.PrimaryContactRelationshiptoClient__c = record.Relationship_to_Client__c;
                    HCPVm.PrimaryContactPhone__c = record.PrimaryContactPhone__c;
                    HCPVm.PrimaryContactPersonVisit__c = record.PersonsRequiredAtVisit__c;
                    HCPVm.ReasonforReferral__c = record.Reason_for_Referral__c;
                    HCPVm.DifferentNameofReferrer__c = record.Name_of_Referrer_Secondary__c;
                    HCPVm.DifferentPositionTitle__c = record.Position_Title_Secondary__c;
                    HCPVm.DifferentOrganisation__c = record.Organization_Secondary__c;
                    HCPVm.DifferentReferrerEmail__c = record.Email_Secondary__c;
                    HCPVm.DifferentReferrerphone__c = record.Phone_Secondary__c;

                    HCPVm.InvoiceName__c = record.Inv_Name__c;
                    HCPVm.InvoiceAddress__c = record.Inv_Address__c;
                    HCPVm.InvoiceEmail__c = record.Inv_Email__c;
                    HCPVm.InvoicePhone__c = record.Inv_Phone__c;

                    HCPVm.SocialInformation__c = record.Social_Information__c;
                    HCPVm.CultureInformation__c = record.Cultural_Requirements__c;
                    HCPVm.BehaviouralIssues__c = record.Behavioural_Issues__c;
                    HCPVm.Pets__c = record.Pets__c;
                    HCPVm.EnviromentalIssues__c = record.Manual_handling_issues__c;
                    HCPVm.OnSitePark__c = record.Onsite_Parking__c;
                    HCPVm.Notes__c = record.Notes__c;
                    HCPVm.Smoker__c = record.Smoker__c;
                    HCPVm.OtherRisks__c = record.Other_risks_alerts__c; 
                    HCPVm.Status__c = record.Status__c;

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
                SFConnect.client.Update("HCP_Job__c", JobId, new
                {                  
                    OT__c = Userinfo.SalesforceID,
                    Date_Allocated__c = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK")),
                    Status__c = "Assigned"
                }) ;

                responseHeader = "Success";
                responseBody = "This job is successfully added to your jobs list";
                responseDialogVisibility = true;
                IsloaderShow = false;
                IsSpinner = false;
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

        // Master Salve dropdown for Home care package
        public bool IsHomeCareLevel { get; set; }
        public void OnChangeFundingTypes(ChangeEventArgs e)
        {
            if (e.Value.ToString() == "Home Care Package")
            {
                IsHomeCareLevel = true;
                HCPVm.FundingTypes__c = e.Value.ToString();
            }
            else
            {
                IsHomeCareLevel = false;
                HCPVm.FundingTypes__c = e.Value.ToString();
            }

        }
    }
}
