using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using SalesforceSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ArdantOffical.Pages.ArdantForms.Components
{
    public partial class DVAReferralForm
    {
        ReferralFormDVAVm DVAVm = new ReferralFormDVAVm();
        public string ErrorMessage { get; set; }
        public bool IsloaderShow { get; set; } = false;
        [Inject]
        public NavigationManager NavManager { get; set; }


        public async Task SaveDVAFormData()
        {
            IsloaderShow = true;
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
                // create a record using an anonymous class and returns the ID
                string result = SFConnect.client.Create("DVA_Jobs__c",
                    new
                    {
                        Refferal__c=DVAVm.ReferralType,
                        Surname__c=DVAVm.PatientSurName,
                        Given_Name__c=DVAVm.PatientGivenName,
                        DVA_FileNo__c=DVAVm.PatientDVAFormNumber,
                        Dob__c =DVAVm.PateintDOB,
                        Age__c=DVAVm.PatientAge,
                        Address__c=DVAVm.PatientAddress,
                        Postcode__c=DVAVm.PatientPostCode,
                        Email__c=DVAVm.PatientEmailAddress,
                        Phone__c=DVAVm.PatientPhoneNumber,
                        Mobile__c=DVAVm.PatientMobileNumber,
                        Card_Type__c=DVAVm.PatientCardType,
                        Accepted_Disabilities__c=DVAVm.PatientAcceptedDisabilities,
                        ReferralTo_Name__c=DVAVm.ReferraltoName,
                        ReferralTo_Address__c=DVAVm.ReferraltoAddress,
                        ReferralTo_Postcode__c=DVAVm.ReferraltoPostCode,
                        ReferralTo_Email__c=DVAVm.ReferraltoEmailAddress,
                        ReferralTo_Phone__c=DVAVm.ReferraltoPhoneNumber,
                        ReferralTo_Mobile__c=DVAVm.ReferraltoMobileNumber,
                        Condition_to_be_treated__c=DVAVm.ConditionTreated,
                        Is_RACF_Patient__c=DVAVm.IsPatientResidentialAged==true?"Yes" : "No",
                        Class_of_care__c=DVAVm.CarePatientClass,
                        Date_funding_began__c=DVAVm.CarePatientDateFundingBegan,
                        Clinical_Details__c=DVAVm.ClinicalDetailsofCondition,
                        Period_of_Referral__c=DVAVm.PeriodofReferral,
                        Other_treating_health_providers__c=DVAVm.OtherTreatingHealthProviders,
                        Provider_Name__c=DVAVm.ProviderName,
                        Provider_Number__c=DVAVm.ProviderNumber,
                        Practice_Name__c=DVAVm.ProviderPracticeName,
                        Practice_Address__c=DVAVm.ProviderPracticeAddress,
                        Practice_Email__c=DVAVm.ProviderEmailAddress,
                        Practice_Phone__c=DVAVm.ProviderPhoneNumber,
                        Practice_Postcode__c=DVAVm.ProviderPostCode,
                        Fax_Number__c=DVAVm.ProviderFaxNumber,
                        OT__c= "a01IR00001eiS2OYAU",
                        Status__c="Open"
                        
                       
                    });

                IsloaderShow = false;
                // ErrorMessage = "Thank you for your inquiry! We will get back to you within 48 hours.";
                NavManager.NavigateTo("/Confirmation");

            }
            catch (Exception ex)
            {
                IsloaderShow = false;
                ErrorMessage = string.Format("Failed to create record: " + ex.Message);

            }
        }

       
        private void AssignReferralType(ChangeEventArgs e)
        {
            if (e.Value != null)
            {
                DVAVm.ReferralType = e.Value.ToString();
            }
        }
        private void AssignPatientCardType(ChangeEventArgs e)
        {
            if (e.Value != null)
            {
                DVAVm.PatientCardType = e.Value.ToString();
            }
           
        }


    }
}
