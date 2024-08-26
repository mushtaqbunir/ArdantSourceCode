using DocumentFormat.OpenXml.Presentation;
using Radzen.Blazor.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.OT
{
    public class ReferralFormHCPVm
    {
        
        // New Fields of HCP
        [Required(ErrorMessage = "This field is required")]
        public string FirstName__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string SureName__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Title__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
   
        public DateTime? DOB__c { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        public string Gender_ShortName { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Street_Address__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Suburb__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Postcode__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string State__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Phone__c { get; set; }
        public string Language__c { get; set; }
        public string MedicalBackground__c { get; set; }
        public string AllergiesorAlerts__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string FundingTypes__c { get; set; }
        public string HomeCarePackageLevel__C { get; set; }
        public string NameofReferrer__c { get; set; }
        public string PositionTitle__c { get; set; }
        public string Organisation__c { get; set; }
        public string ReferrerEmail__c { get; set; }
        public string ReferrerContactphone__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string PrimaryContactFullName__c { get; set; }
        public string PrimaryContactRelationshiptoClient__c { get; set; }
        public string PrimaryContactPhone__c { get; set; }
        public string PrimaryContactPersonVisit__c { get; set; }

        public string DifferentNameofReferrer__c { get; set; }
        public string DifferentPositionTitle__c { get; set; }
        public string DifferentOrganisation__c { get; set; }
        public string DifferentReferrerEmail__c { get; set; }
        public string DifferentReferrerphone__c { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string ReasonforReferral__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string InvoiceName__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string InvoiceAddress__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string InvoiceEmail__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string InvoicePhone__c { get; set; }
        public string SocialInformation__c { get; set; }
        public string CultureInformation__c { get; set; }
        public string BehaviouralIssues__c { get; set; }
        public string Pets__c { get; set; }
        public string EnviromentalIssues__c { get; set; }
        public string OnSitePark__c { get; set; }
        public string Notes__c { get; set; }
        public string Smoker__c { get; set; }
        public string OtherRisks__c { get; set; }
        public string Job_Number__c { get; set; }
        public string Date_Allocated__c { get; set; }
        public string Status__c { get; set; }
    }
}
