using ArdantOffical.Data.ModelVm.ClinicalData;
using ArdantOffical.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ArdantOffical.Data.ModelVm.Invoices
{
    public class InvoicesVM
    {
        public string Id { get; set; }
        public string Title { get; set; } = "Invoice";
        [Required(ErrorMessage = "Invoice date is required")]
        public DateTime? InvoiceDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "The email address is not valid")]
        public string SentTo { get; set; }

        public DateTime? DueDate { get; set; } = DateTime.Now.AddDays(14);
        public string Status { get; set; }
        public decimal InvoiceTotal { get; set; }
        public string Job_Number { get; set; }
        public string Customer_Name { get; set; }
        public string CustomerPhone { get; set; }
        public string  NDISNumber { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public string OT { get; set; }
        public string Folder { get; set; }
        public string Filename { get; set; }
        public string JobID { get; set; }
        public string InvoiceNo { get; set; }
        public string AHPRANo { get; set; }
        public decimal? SubTotal { get => InvoiceItemList.Sum(x => x.Amount); }
        public decimal? TaxAmount { get => InvoiceItemList.Sum(x => x.TaxAmount); }
        public decimal? Total { get => InvoiceItemList.Sum(x => x.TotalAmount); }
        public string TaxRate { get => InvoiceItemList.Select(ii => ii.Tax).FirstOrDefault(); }
        [EnsureOneElement(ErrorMessage = "At least one item is required")]
        public List<InvoiceItemsVm> InvoiceItemList { get; set; } = new();
        public string OTName { get; set; }
        public string OTEmail { get; set; }
        public string Bank_Account_Number__c { get; set; }
        public string Bank_Account_BSB__c { get; set; }
        public string ABN { get; set; }
    }

    public class InvoicesVMForTable
    {
        public List<InvoicesVM> Invoices { get; set; }
        public List<InvoicesVM> Invoices_All { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
