using System.ComponentModel.DataAnnotations;
using System;

namespace ArdantOffical.Data.ModelVm
{
    public class SearchParamsVm
    {

        // [Required(ErrorMessage = "Start Date is required")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }
        //[Required(ErrorMessage = "End Date is required")]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "Transaction Status is required")]
        [Display(Name = "Transaction Status")]
        public string TransactionStatus { get; set; }
        [Required(ErrorMessage = "Transaction Type is required")]
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }
        public string FlagType { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }
        [Required(ErrorMessage = "Please select user")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select user")]

        public int UserID { get; set; }

        // SAR Properties
        public string SarType { get; set; }
        public string ReferredToNCA { get; set; }
        public string SearchKey { get; set; }
        public string TransactionCategory { get; set; }
        public string Amountcurrency { get; set; }
        public string AmountFromRange { get; set; }

        public string AmountTolRange { get; set; }
        public int ClientID { get; set; }
        public string ClientIDStr { get; set; }
        public string Bank { get; set; }

    }

    public class SearchParamsTop
    {

        [Required(ErrorMessage = "Start Date is required")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "End Date is required")]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; } = DateTime.Now;
    }
}

