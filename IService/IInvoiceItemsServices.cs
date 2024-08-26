using ArdantOffical.Data.ModelVm.Invoices;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ArdantOffical.Models;

namespace ArdantOffical.IService
{
    public interface IInvoiceItemsServices
    {
        Task<Exception> AddInvoiceItem(InvoiceItemsVm Model);
        Task<Exception> DeleteInvoiceItem(string Id);
        Task<InvoiceItemsVm> GetInvoiceByID(string Id);
        Task<InvoiceItemsVm> GetInvoiceItem(string invoiceId);
        public Task<InvoiceItemsVm> GetInvoiceItems(string invoiceId);
        Task<Exception> UpdateInvoiceItem(InvoiceItemsVm Model);
        Task<List<SelectListItem>> GetAssignedJobs();
        Task<List<NDIS>> GetAssignedNDISJobs();
        Task<Exception> AddInvoiceItem(List<InvoiceItemsVm> Items);
    }
}
