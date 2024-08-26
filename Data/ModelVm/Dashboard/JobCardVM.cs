using System;

namespace ArdantOffical.Data.ModelVm.Dashboard
{
    public class JobCardVM
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime?  DatePosted { get; set; }
        public string Gender { get; set; }
        public string Postcode { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Age { get; set; }
        public string DiagnosedConditions { get; set; }
        public string ShortDC { get; set; }

    }
}
