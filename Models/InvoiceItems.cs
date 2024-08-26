using System;

namespace ArdantOffical.Models
{
    public class InvoiceItems
    {
        public string Id { get; set; }
        public decimal? Amount__c { get; set; }
        public DateTime? Date__c { get; set; }
        public string Description__c { get; set; }
        public string Item__c { get; set; }
        public string NDIS_Invoice__c { get; set; }
        public decimal? Qty_Hours__c { get; set; }
        public decimal? Qty_KMs__c { get; set; }
        public decimal? Qty_Minutes__c { get; set; }
        public string Quantity_Time__c { get; set; }
        public string Rate__c { get; set; }
        public decimal? Tax_Rate__c { get; set; }
        public string Unit__c { get; set; }
        public decimal? Unit_Price__c { get; set; }
    
      
    }
}
