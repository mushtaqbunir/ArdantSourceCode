using System;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.OT
{
    public class ReferralFormDVAVm
    {
        [Required(ErrorMessage = "Referral Type is required")]
        public string  ReferralType { get; set; }
        [Required(ErrorMessage ="Surname is required")]
        public string PatientSurName { get; set; }
        [Required(ErrorMessage = "Given Name is required")]
        public string PatientGivenName { get; set; }
        [Required(ErrorMessage = "DVA Form Number is required")]
        public string PatientDVAFormNumber { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string PatientAddress { get; set; }
        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime? PateintDOB { get; set; }
        public string PatientAge {
            get
            {
                var currentDate = DateTime.Now.Year;
                string _age = string.Empty;
               
                if (PateintDOB != null)
                {
                    _age = Convert.ToString(currentDate - PateintDOB.Value.Year);
                    _age = _age + " " + "years";
                }
                return _age;
            }
        }
        [Required(ErrorMessage = "Post Code is required")]
        public string PatientPostCode { get; set; }
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address Address")]
        public string PatientEmailAddress { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PatientPhoneNumber{ get; set; }
        public string PatientMobileNumber { get; set; }
        [Required(ErrorMessage = "Card Type is required")]
        public string PatientCardType { get; set; }
        [Required(ErrorMessage = "Accepted Disabilities Address is required")]
        public string PatientAcceptedDisabilities { get; set; }
        public string ReferraltoName { get; set; }
        public string ReferraltoAddress { get; set; }
        public string ReferraltoPostCode { get; set; }
        public string ReferraltoEmailAddress { get; set; }
        public string ReferraltoPhoneNumber { get; set; }
        public string ReferraltoMobileNumber { get; set; }
        public string ReferraltoProviderNumber { get; set; }
        [Required(ErrorMessage = "Please mention primary condition to be treated")]
        public string ConditionTreated { get; set; }
        public bool IsPatientResidentialAged { get; set; }
        public string CarePatientClass { get; set; }
        public DateTime? CarePatientDateFundingBegan { get; set; }
        public string ClinicalDetailsofCondition { get; set; }
        public string PeriodofReferral { get; set; }
        public string OtherTreatingHealthProviders { get; set; }
        [Required(ErrorMessage = "Provider Name is required")]
        public string ProviderName { get; set; }
        [Required(ErrorMessage = "Provider Number is required")]
        public string ProviderNumber { get; set; }
        [Required(ErrorMessage = "Practice Name is required")]
        public string ProviderPracticeName { get; set; }
        [Required(ErrorMessage = "Practice Address is required")]
        public string ProviderPracticeAddress { get; set; }
        [Required(ErrorMessage = "Provider Post Code is required")]
        public string ProviderPostCode { get; set; }
        [Required(ErrorMessage = "Provider Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address Address")]
        public string ProviderEmailAddress { get; set; }
        [Required(ErrorMessage = "Provider Phone Number is required")]
        public string ProviderPhoneNumber { get; set; }
        public string ProviderFaxNumber { get; set; }
        public string ProviderSignature { get; set; }
        public string Job_Number__c { get; set; }
        public string Date_Allocated__c { get; set; }
        public string Status__c { get; set; }
        //[Required(ErrorMessage = "Provider Date is required")]
        //public DateTime? ProviderDate { get; set; }

    }
 
}
