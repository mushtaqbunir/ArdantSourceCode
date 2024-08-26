using System.Collections.Generic;

#nullable disable

namespace ArdantOffical.Data.ModelVm.RulesVM
{
    public class RulesVM
    {
        public int ID { get; set; }
        public string CodeName { get; set; }
        public string RuleName { get; set; }
        public int Score { get; set; }
        public bool Status { get; set; }
        public string Category { get; set; }
        public int? Amount { get; set; }
        public int? Duration { get; set; }
        public string DurationType { get; set; }
        public int? MultiplyFactor { get; set; }
        public int? PercentageFactor { get; set; }
        public int? RangeFrom { get; set; }
        public int? RangeTo { get; set; }


    }
    public class RulesVMUseForTable
    {
        public List<RulesVM> RulesVMList { get; set; }
        public List<RulesVM> RulesVMListUseForSearch { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
    public class RulesPagination
    {
        public List<RulesVM> PerPageRecords { get; set; }
        public List<RulesVM> AllRecords { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
