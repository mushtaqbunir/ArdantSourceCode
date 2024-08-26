using System;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Models
{
    public class APIErrorLog
    {
        [Key]
        public int ID { get; set; }
        public string TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
    }
}
