using SFService;
using System;

namespace ArdantOffical.Models
{
    public class HCP
    {
        public string Id { get; set; }
        public string Job_Number__c { get; set; }
        public string P_Firstname__c { get; set; }
        public string P_Surname__c { get; set; }
        public string Title__c { get; set; }
        public string Gender_Abbrev__c { get; set; }
        public string Gender__c { get; set; }
        public DateTime? Date_of_Birth__c { get; set; }
        public string P_Address__c { get; set; }
        public string Street_Address__c { get; set; }
        public string Suburb__c { get; set; }
        public string Postcode__c { get; set; }
        public string State__c { get; set; }
        public string P_Phone__c { get; set; }
        public string Language__c { get; set; }
        public string Medical_Background__c { get; set; }
        public string Funding_Type__c { get; set; }
        public string Funding_Sub_Type__c { get; set; }
        public string Allergies_or_Alerts__c { get; set; }

        public string Name_of_Referrer__c { get; set; }
        public string Position_Title__c { get; set; }
        public string Organization__c { get; set; }
        public string Coordinator_Email__c { get; set; }
        public string Coordinator_Phone__c { get; set; }

        public string PrimaryContact_Name__c { get; set; }
        public string Relationship_to_Client__c { get; set; }
        public string PrimaryContactPhone__c { get; set; }
        public string PersonsRequiredAtVisit__c { get; set; }

        public string Reason_for_Referral__c { get; set; }

        public string Name_of_Referrer_Secondary__c { get; set; }
        public string Position_Title_Secondary__c { get; set; }
        public string Organization_Secondary__c { get; set; }
        public string Email_Secondary__c { get; set; }
        public string Phone_Secondary__c { get; set; }

        public string Inv_Name__c { get; set; }
        public string Inv_Email__c { get; set; }
        public string Inv_Phone__c { get; set; }
        public string Inv_Address__c { get; set; }
        public string Social_Information__c { get; set; }
        public string Cultural_Requirements__c { get; set; }
        public string Behavioural_Issues__c { get; set; }

        public string Pets__c { get; set; }
        public string Manual_handling_issues__c { get; set; }

        public string Onsite_Parking__c { get; set; }
        public string Notes__c { get; set; }
        public string Smoker__c { get; set; }
        public string Other_risks_alerts__c { get; set; }
        public string OT__c { get; set; }
        public string Status__c { get; set; } = "Open";

        public DateTime? CreatedDate { get; set; }
        public DateTime? Date_Allocated__c { get; set; }
    }
}
