using System;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Models
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime PostedDate { get; set; }
        public string PostedBy { get; set; }
        public int PostedById { get; set; }
        public string LastModifiedBy { get; set; }
        public int LastModifiedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
