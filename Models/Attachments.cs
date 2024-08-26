using System;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Models
{
    public class Attachments
    {
        [Key]
        public int ID { get; set; }
        public string Type { get; set; }
        public string SalesforceID { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Folder { get; set; }
        public string PostedBy { get; set; }
        public DateTime?  DatePosted { get; set; }
        public bool IsDeleted { get; set; }

    }
}
