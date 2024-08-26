using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Invoices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArdantOffical.IService
{
    public interface IInvoiceServices
    {
        Task<string> AddInvoice(InvoicesVM Model, string JobId,string htmlcontent);
        public Task<string> UpdateInvoice(InvoicesVM Model, string JobId, string htmlcontent);
       // Task<Exception> DeleteInvoice(string Id);
        Task<InvoicesVM> GetInvoiceByID(string Id);
        public Task<InvoicesVM> GetInvoiceItems(string Id);
        Task<InvoicesVMForTable> GetInvoices(string JobId);
        Task<InvoicesVMForTable> GetInvoiceSearch([FromQuery] PaginationDTO pagination, SearchFilterVm searchFilter, string OTId);
        Task<InvoicesVMForTable> GetNDISInvoices([FromQuery] PaginationDTO pagination, string OTId);
        Task<InvoicesVMForTable> GetNDISInvoices([FromQuery] PaginationDTO pagination);
        public Task<List<SelectListItem>> GetAllOTs();
        Task<Exception> UpdateInvoice(InvoicesVM Model, string Id);
        public Task DeleteInvoice(string id);
        Task<string> GetInvoiceHtmlContent(string baseUrl,string invoiceId);
        public byte[] CreateReconciliationSheet(List<InvoicesVM> lstExport);
        Task<Exception> SendInvoiceInEmail(string htmlContent, string filename, InvoicesVM InvoiceModal);
        List<SelectListItem> GetTaxesList();
        List<SelectListItem> GetRateList();
    }
}
