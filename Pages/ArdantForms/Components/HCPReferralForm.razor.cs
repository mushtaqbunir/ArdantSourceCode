using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components;
using SalesforceSharp;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Blazorise;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;

namespace ArdantOffical.Pages.ArdantForms.Components
{
    public partial class HCPReferralForm
    {
        ReferralFormHCPVm HCPVm = new ReferralFormHCPVm(); 
        public string ErrorMessage { get; set; }
        public bool IsloaderShow { get; set; } = false;
        [Inject]
        public NavigationManager NavManager { get; set; }
        public List<SelectListItem> FundingTypes = new List<SelectListItem>();
        public List<SelectListItem> lstStates = new List<SelectListItem>();
        public bool IsSpinner { get; set; }


        protected override void OnInitialized()
        {
            try
            {
                FundingTypesList();
                GetStates();
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
    
        public  void FundingTypesList()
        {
            FundingTypes.Add(new SelectListItem() { Value = "Home Care Package", Text = "Home Care Package" });
            FundingTypes.Add(new SelectListItem() { Value = "CHSP", Text = "CHSP" });
            FundingTypes.Add(new SelectListItem() { Value = "Transition Care Package", Text = "Transition Care Package" });
            FundingTypes.Add(new SelectListItem() { Value = "Privately funded", Text = "Privately funded" });
            FundingTypes.Add(new SelectListItem() { Value = "Other", Text = "Other" });
        }

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
        public async Task Save()
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
                await Task.Delay(10);
                // create a record using an anonymous class and returns the ID
                //HCP HCPModal = new HCP
                //{
                //    Title__c = HCPVm.Title__c,
                //    P_Firstname__c = HCPVm.FirstName__c,
                //    P_Surname__c = HCPVm.SureName__c,
                //    Date_of_Birth__c = HCPVm.DOB__c,
                //    P_Address__c = HCPVm.Address__c,
                //    P_Phone__c = HCPVm.Phone__c,
                //    Language__c = HCPVm.Language__c,
                //    Medical_Background__c = HCPVm.MedicalBackground__c,
                //    Allergies_or_Alerts__c = HCPVm.AllergiesorAlerts__c,
                //    Funding_Type__c = HCPVm.FundingTypes__c,
                //    Funding_Sub_Type__c = HCPVm.HomeCarePackageLevel__C,

                //    Name_of_Referrer__c = HCPVm.NameofReferrer__c,
                //    Position_Title__c = HCPVm.PositionTitle__c,
                //    Organization__c = HCPVm.Organisation__c,
                //    Coordinator_Email__c = HCPVm.ReferrerEmail__c,
                //    Coordinator_Phone__c = HCPVm.ReferrerContactphone__c,

                //    PrimaryContact_Name__c = HCPVm.PrimaryContactFullName__c,
                //    Relationship_to_Client__c = HCPVm.PrimaryContactRelationshiptoClient__c,
                //    PrimaryContactPhone__c = HCPVm.PrimaryContactPhone__c,
                //    PersonsRequiredAtVisit__c = HCPVm.PrimaryContactPersonVisit__c,

                //    Reason_for_Referral__c = HCPVm.ReasonforReferral__c,
                //    Name_of_Referrer_Secondary__c = HCPVm.DifferentNameofReferrer__c,
                //    Position_Title_Secondary__c = HCPVm.DifferentPositionTitle__c,
                //    Organization_Secondary__c = HCPVm.DifferentOrganisation__c,
                //    Email_Secondary__c = HCPVm.DifferentReferrerEmail__c,
                //    Phone_Secondary__c = HCPVm.DifferentReferrerphone__c,

                //    Inv_Name__c = HCPVm.InvoiceName__c,
                //    Inv_Address__c = HCPVm.InvoiceAddress__c,
                //    Inv_Email__c = HCPVm.InvoiceEmail__c,
                //    Inv_Phone__c = HCPVm.InvoicePhone__c,

                //    Social_Information__c = HCPVm.SocialInformation__c,
                //    Cultural_Requirements__c = HCPVm.CultureInformation__c,
                //    Behavioural_Issues__c = HCPVm.BehaviouralIssues__c,
                //    Pets__c = HCPVm.Pets__c,
                //    Manual_handling_issues__c = HCPVm.EnviromentalIssues__c,
                //    Onsite_Parking__c = HCPVm.OnSitePark__c,
                //    Notes__c = HCPVm.Notes__c,
                //    Smoker__c = HCPVm.Smoker__c,
                //    Other_risks_alerts__c = HCPVm.OtherRisks__c,
                //    OT__c = "a01IR00001eiS2OYAU",
                //    Status__c = "Open"
                //};

                // create a record using an anonymous class and returns the ID
                string GenderShortName = string.Empty;
                switch (HCPVm.Gender)
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
                string result = SFConnect.client.Create("HCP_Job__c", new 
                {
                    Title__c = HCPVm.Title__c,
                    P_Firstname__c = HCPVm.FirstName__c,
                    P_Surname__c = HCPVm.SureName__c,
                    Date_of_Birth__c = HCPVm.DOB__c,
                    Gender__c = HCPVm.Gender,
                    Gender_Abbrev__c = GenderShortName,
                    Street_Address__c = HCPVm.Street_Address__c,
                    Suburb__c = HCPVm.Suburb__c,
                    Postcode__c = HCPVm.Postcode__c,
                    State__c = HCPVm.State__c,
                    P_Phone__c = HCPVm.Phone__c,
                    Language__c = HCPVm.Language__c,
                    Medical_Background__c = HCPVm.MedicalBackground__c,
                    Allergies_or_Alerts__c = HCPVm.AllergiesorAlerts__c,
                    Funding_Type__c = HCPVm.FundingTypes__c,
                    Funding_Sub_Type__c = HCPVm.HomeCarePackageLevel__C,

                    Name_of_Referrer__c = HCPVm.NameofReferrer__c,
                    Position_Title__c = HCPVm.PositionTitle__c,
                    Organization__c = HCPVm.Organisation__c,
                    Coordinator_Email__c = HCPVm.ReferrerEmail__c,
                    Coordinator_Phone__c = HCPVm.ReferrerContactphone__c,

                    PrimaryContact_Name__c = HCPVm.PrimaryContactFullName__c,
                    Relationship_to_Client__c = HCPVm.PrimaryContactRelationshiptoClient__c,
                    PrimaryContactPhone__c = HCPVm.PrimaryContactPhone__c,
                    PersonsRequiredAtVisit__c = HCPVm.PrimaryContactPersonVisit__c,

                    Reason_for_Referral__c = HCPVm.ReasonforReferral__c,
                    Name_of_Referrer_Secondary__c = HCPVm.DifferentNameofReferrer__c,
                    Position_Title_Secondary__c = HCPVm.DifferentPositionTitle__c,
                    Organization_Secondary__c = HCPVm.DifferentOrganisation__c,
                    Email_Secondary__c = HCPVm.DifferentReferrerEmail__c,
                    Phone_Secondary__c = HCPVm.DifferentReferrerphone__c,

                    Inv_Name__c = HCPVm.InvoiceName__c,
                    Inv_Address__c = HCPVm.InvoiceAddress__c,
                    Inv_Email__c = HCPVm.InvoiceEmail__c,
                    Inv_Phone__c = HCPVm.InvoicePhone__c,

                    Social_Information__c = HCPVm.SocialInformation__c,
                    Cultural_Requirements__c = HCPVm.CultureInformation__c,
                    Behavioural_Issues__c = HCPVm.BehaviouralIssues__c,
                    Pets__c = HCPVm.Pets__c,
                    Manual_handling_issues__c = HCPVm.EnviromentalIssues__c,
                    Onsite_Parking__c = HCPVm.OnSitePark__c,
                    Notes__c = HCPVm.Notes__c,
                    Smoker__c = HCPVm.Smoker__c,
                    Other_risks_alerts__c = HCPVm.OtherRisks__c,
                    OT__c = "a01IR00001eiS2OYAU",
                    Status__c = "Open"
                });
                IsSpinner = false;
                NavManager.NavigateTo("/Confirmation");

            }
            catch (Exception ex)
            {
                IsloaderShow = false;
                ErrorMessage = string.Format("Failed to create record: " + ex.Message);

            }
        }

       
       
        //private void SelectGender(ChangeEventArgs e)
        //{
        //    if (e.Value != null)
        //    {
        //        HCPVm.Gender = e.Value.ToString();
        //    }
        //}
      
        
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

        public void OnChangeState(ChangeEventArgs e)
        {           
          
            HCPVm.State__c = e.Value.ToString();           

        }
    }
}
