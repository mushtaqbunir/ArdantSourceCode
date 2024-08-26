using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Helpers;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Models;
using ArdantOffical.Services;
using ClosedXML;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using SalesforceSharp;
using SFService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using Task = System.Threading.Tasks.Task;


namespace ArdantOffical.Pages.ArdantForms.Components
{
    public partial class NDISReferralForm
    {
        public bool IsSupportCoordinator { get; set; }

        public List<SelectListItem> lstPrimaryAllied = new();

        public ReferralFormNDISVm NDISVm = new ReferralFormNDISVm();
        public string ErrorMessage { get; set; }
        public bool IsloaderShow { get; set; } = false;
        public List<SelectListItem> lstStates = new List<SelectListItem>();
        public List<SelectListItem> lstDays = new List<SelectListItem>();
        public List<SelectListItem> lstMonths = new List<SelectListItem>();
        public List<SelectListItem> lstYears = new List<SelectListItem>();
        [Inject]
        public NavigationManager NavManager { get; set; }

        public bool IsSpinner { get; set; }
        public void GetStates()
        {
            lstStates.Add(new SelectListItem() { Value = "QLD", Text = "QLD" });
            lstStates.Add(new SelectListItem() { Value = "VIC", Text = "VIC" });
            lstStates.Add(new SelectListItem() { Value = "NSW", Text = "NSW" });
            lstStates.Add(new SelectListItem() { Value = "SA", Text = "SA" });
            lstStates.Add(new SelectListItem() { Value = "WA", Text = "WA" });
            lstStates.Add(new SelectListItem() { Value = "TAS", Text = "TAS" });
            lstStates.Add(new SelectListItem() { Value = "ACT", Text = "ACT" });
            lstStates.Add(new SelectListItem() { Value = "NT", Text = "NT" });
        }

        public void GetMonths()
        {
            lstMonths.Add(new SelectListItem() { Value = "1", Text = "January" });
            lstMonths.Add(new SelectListItem() { Value = "2", Text = "Feburary" });
            lstMonths.Add(new SelectListItem() {Value = "3", Text = "March" });
            lstMonths.Add(new SelectListItem() {Value = "4", Text = "April" });
            lstMonths.Add(new SelectListItem() {Value = "5", Text = "May" });
            lstMonths.Add(new SelectListItem() {Value = "6", Text = "June" });
            lstMonths.Add(new SelectListItem() {Value = "7", Text = "July" });
            lstMonths.Add(new SelectListItem() {Value = "8", Text = "August" });
            lstMonths.Add(new SelectListItem() {Value = "9", Text = "September" });
            lstMonths.Add(new SelectListItem() {Value = "10", Text = "October" });
            lstMonths.Add(new SelectListItem() {Value = "11", Text = "November" });
            lstMonths.Add(new SelectListItem() {Value = "12", Text = "December" });
        }

        public void GetDays(List<SelectListItem> _targetList)
        {
            for(int i=1; i<=31; i++)
            {
                _targetList.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString() });
            }
        }

        public void GetYears(List<SelectListItem> _targetList)
        {
            int StartYear = DateTime.Now.Year-100;
            int CurrentYear = DateTime.Now.Year;
            for (int i= CurrentYear; i>= StartYear; i--)
            {
                _targetList.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString() });
            }
          
        }

    

        protected override async Task OnInitializedAsync()
        {
            try
            {
                GetStates();
                GetDays(lstDays);
                GetMonths();
                GetYears(lstYears);
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
        }
        public async Task SaveNDISFormData()
        {
           
         
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            // all actions should be in a try-catch - i'll just do the authentication one for an example
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
            // Call the create method to create the record
            try
            {

                IsSpinner = true;
                // create a record using an anonymous class and returns the ID
                string GenderShortName = string.Empty;
               switch(NDISVm.Gender)
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
                string result = SFConnect.client.Create("NDIS_Job__c", new 
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
                    Gender__c=NDISVm.Gender,
                    Gender_Abbrev__c=GenderShortName,
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
                    PrimaryContactEmail__c=NDISVm.PrimaryContactEmail__c,
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
                    OT__c = "a01IR00001eiS2OYAU",
                    Status__c = "Open"
                });
                IsSpinner = false;
                // ErrorMessage = "Thank you for your inquiry! We will get back to you within 48 hours.";
                NavManager.NavigateTo("/Confirmation");

            }
            catch (Exception ex)
            {
                IsSpinner = false;
                ErrorMessage = string.Format("Failed to create record: " + ex.Message);
            }
        }
        public void GetPrimaryAlliedHealth()
        {
            lstPrimaryAllied.Add(new SelectListItem() { Value = "Occupational Therapy", Text = "Occupational Therapy" });
            //lstPrimaryAllied.Add(new SelectListItem() { Value = "Speech Pathology", Text = "Speech Pathology" });
            //lstPrimaryAllied.Add(new SelectListItem() { Value = "Physiotherapy", Text = "Physiotherapy" });
            //lstPrimaryAllied.Add(new SelectListItem() { Value = "Behaviour Support", Text = "Behaviour Support" });
            //lstPrimaryAllied.Add(new SelectListItem() { Value = "Early Childhood Practitioner: Key Worker", Text = "Early Childhood Practitioner: Key Worker" });
        }
     
        public bool IsSelfManagedPlan { get; set; }
        public void BillingCheckBox(string value)
        {
            IsSelfManagedPlan = value == NDISCheckboxes.selfManaged;
            //if (NDISVm.PlanManaged.Contains(value))
            //{
            //    NDISVm.PlanManaged.Remove(value);
            //}
            //else
            //{
            //    NDISVm.PlanManaged.Add(value);
            //}
        }

       
        public string yesChecked = "";
        public string noChecked = "";
        public bool IsPrimaryContact { get; set; }
        public void VisbilityPrimaryContact()
        {
            yesChecked = "";
            noChecked = "checkbox";
            IsPrimaryContact = true;
        }
        public void HideVisbilityPrimaryContact()
        {
            yesChecked = "checkbox";
            noChecked = "";
            IsPrimaryContact = false;
        }
        public void OnChangeDay(ChangeEventArgs e)
        {
            NDISVm.Day = e.Value.ToString();
        }

        public void OnChangeMonth(ChangeEventArgs e)
        {
            NDISVm.Month = e.Value.ToString();
        }

        public void OnChangeYear(ChangeEventArgs e)
        {
            NDISVm.Year = e.Value.ToString();
        }

        public void OnChangeState(ChangeEventArgs e)
        {
            NDISVm.State__c = e.Value.ToString();
        }

        //private void SelectGender(ChangeEventArgs e)
        //{
        //    if (e.Value != null)
        //    {
        //        NDISVm.Gender = e.Value.ToString();
        //    }
        //}
        //private void IdentifyAboriginalORTorresStrait(ChangeEventArgs e)
        //{
        //    if (e.Value != null)
        //    {
        //        NDISVm.ParticipantAboriginal = e.Value.ToString();
        //    }
        //}
        //private void LivingArrangements(ChangeEventArgs e)
        //{
        //    if (e.Value != null)
        //    {
        //        NDISVm.ParticiLivingArrangement = e.Value.ToString();
        //    }
        //}
        //private void MethodofContact(ChangeEventArgs e)
        //{
        //    if (e.Value != null)
        //    {
        //        NDISVm.Methodofcontact = e.Value.ToString();
        //    }
        //}
    }
}
