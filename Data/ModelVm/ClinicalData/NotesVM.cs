using ArdantOffical.Data.ModelVm.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.ClinicalData
{
    public class NotesVM
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Note title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class NotesVMForTable
    {
        public List<NotesVM> Notes { get; set; }
        public List<NotesVM> Notes_All { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
