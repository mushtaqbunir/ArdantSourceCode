using System;

namespace ArdantOffical.Models
{
    public class Invoice
    {
        public string Id { get; set; }
        public string Invoice_Title__c { get; set; }
        public DateTime? Invoice_Date__c { get; set; }
        public DateTime? Due_Date__c { get; set; }
        public string Invoice_Email_To__c { get; set; }
        public string Filename__c { get; set; }
        public string Invoice_Filepath__c { get; set; }
        public string Status__c { get; set; }
        public string NDIS_Job__c { get; set; }
        public string Job_Number__c { get; set; }
        public string Customer_Name__c { get; set; }
        public string OT__c { get; set; }
        public decimal Invoice_Total__c { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Inv_Number__c { get; set; }
        public string AHPRA_No__c { get; set; }
        public string ABN__c { get; set; }
        public string FullName__c { get; set; }
        public string Email__c { get; set; }
        public string NDIS_Plan_Number__c { get; set; }
        public string Bank_Account_Number__c { get; set; }
        public string IsDelete__c { get; set; }
        public string Bank_Account_BSB__c { get; set; }
    }
}
