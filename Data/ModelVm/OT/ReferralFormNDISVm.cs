using ArdantOffical.Helpers.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2013.Excel;
using Humanizer;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArdantOffical.Data.ModelVm.OT
{
    public class ReferralFormNDISVm
    {
        // New Fields of NDIS

        [Required(ErrorMessage = "This field is required")]
        public string FirstName__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string SureName__c { get; set; }
     
        public string Title__c { get; set; }      

        public DateTime? DOB__c
        {
            get
            {
                //return FGCExtensions.UsDateTime(DOBStr__c);
                return new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day));
            }
        }
        [Required(ErrorMessage = "Day is required")]
        public string Day { get; set; }
        [Required(ErrorMessage = "Month is required")]
        public string Month { get; set; }
        [Required(ErrorMessage = "Year is required")]
        public string Year { get; set; }

        
        public string DOBStr__c { get; set; }
		[Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        public string Gender_ShortName { get; set; }
  
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
        public string NDISManagementType__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string NDISPlanNumber__c { get; set; }
        public string NameofReferrer__c { get; set; }
        public string PositionTitle__c { get; set; }
        public string Organisation__c { get; set; }
        public string ReferrerEmail__c { get; set; }
        public string ReferrerContactphone__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string PrimaryContactFullName__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string PrimaryContactRelationshiptoClient__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string PrimaryContactPhone__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string PrimaryContactEmail__c { get; set; }
        public string PrimaryContactPersonVisit__c { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string ReasonforReferral__c { get; set; }
        public string DifferentNameofReferrer__c { get; set; }
        public string DifferentPositionTitle__c { get; set; }
        public string DifferentOrganisation__c { get; set; }
        public string DifferentReferrerEmail__c { get; set; }
        public string DifferentReferrerphone__c { get; set; }
     
        public string InvoiceName__c { get; set; }
     
        public string InvoiceAddress__c { get; set; }
      
        public string InvoiceEmail__c { get; set; }
   
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
