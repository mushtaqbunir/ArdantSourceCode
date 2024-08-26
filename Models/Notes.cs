using System;

namespace ArdantOffical.Models
{
    public class Notes
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ParentId { get; set; }
        public DateTime?    CreatedDate { get; set; }
    }

    public class NDISNotes
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description__c { get; set; }
        public string NDIS_Job_ID__c { get; set; }
        public string IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class HCPNotes
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description__c { get; set; }
        public string HCP_Job_ID__c { get; set; }
        public string IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class DVANotes
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description__c { get; set; }
        public string DVA_Job_ID__c { get; set; }
        public string IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
