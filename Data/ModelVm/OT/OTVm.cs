using DocumentFormat.OpenXml.Office2013.Excel;
using System;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.OT
{
    public class OTVm
    {
        public string ID { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Valid Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "AHRPA Number is required")]
        public string AHPRANo { get; set; }
        public string ABN { get; set; }
        public string Message { get; set; }
        public string Fullname__c { get; set; }
        public string IsEmployed__c { get; set; }
        public string Bank_Account_BSB__c { get; set; }
        public string Bank_Account_Number__c { get; set; }
        public string Right__c { get; set; }
        public DateTime? DoB__c { get; set; }
        public string Experience__c { get; set; }
        public string Hours_per_week__c { get; set; }
        public string HearAboutUs__c { get; set; }
        public string Insurance__c { get; set; }
        public string Medicare_Provider_No__c { get; set; }
        public string NDIS_Registered__c { get; set; }
        public string Open_to_Telehealth__c { get; set; }
        public string Preference__c { get; set; }
        public string State__c { get; set; }
        public string StartWorkingDate__c { get; set; }
        public string Willing_to_travel_KM__c { get; set; }
        public string UserStatus { get; set; }
        public string UserRole { get; set; }

    }
}
