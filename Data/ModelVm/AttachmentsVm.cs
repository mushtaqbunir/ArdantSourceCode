using ArdantOffical.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm
{
    public class AttachmentsVm
    {
        [Key]
        public int ID { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string SalesforceID { get; set; }
        [Required(ErrorMessage = "Title is required !")]
        public string Title { get; set; }
        public UserFileType UserFileType { get; set; }
        public string Path { get; set; }
        public List<string> FileNames { get; set; } = new();
        public string Folder { get; set; }
        public string PostedBy { get; set; }
        public DateTime? DatePosted { get; set; }
        public bool IsDeleted { get; set; }
    }
}
