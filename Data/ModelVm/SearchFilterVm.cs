using System;

namespace ArdantOffical.Data.ModelVm
{
    public class SearchFilterVm
    {
        public string SearchKey { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; }
    }
}
