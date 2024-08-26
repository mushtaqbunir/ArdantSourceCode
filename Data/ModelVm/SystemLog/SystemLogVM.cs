using System;
using System.Collections.Generic;

namespace ArdantOffical.Data.ModelVm.SystemLog
{
    public class SystemLogVM
    {
        public int ID { get; set; }
        public string Object { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Remarks { get; set; }
        public string PostedBy { get; set; }
        public int? UserID { get; set; }
        public int? CRID { get; set; }
        public DateTime? DatePosted { get; set; }
        public int? CommentID { get; set; }
        public int? HdID { get; set; }
        public string IP { get; set; }
        public int? ARID { get; set; }


    }

    public class SystemLogPagination
    {
        public List<SystemLogVM> SystemLogPerPage { get; set; }
        public List<SystemLogVM> SystemLogPerPageVmListForsearch { get; set; }
        public List<SystemLogVM> SystemLogAll_Export { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
