using ArdantOffical.Data.ModelVm.ClinicalData;
using ArdantOffical.Data;
using ArdantOffical.Helpers;
using ArdantOffical.IService;
using Microsoft.AspNetCore.Mvc;
using SalesforceSharp;
using System.Linq;
using System.Threading.Tasks;
using System;
using ArdantOffical.Data.ModelVm.Invoices;
using ArdantOffical.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Users;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using DinkToPdf.Contracts;
using DinkToPdf;
using ArdantOffical.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.Office2010.Excel;
using ArdantOffical.Components.JobInvoices;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.VariantTypes;
using ClosedXML.Excel;
using ArdantOffical.Data.ModelVm.OT;

namespace ArdantOffical.Services
{
    public class InvoiceServices:ControllerBase, IInvoiceServices
    {
        public string rootFolder;
        private IConverter _converter;
        private readonly FGCDbContext context;
        private readonly HttpClient _httpClient;
        private IWebHostEnvironment Environment { get; set; }
        public EmailService _emailService;
        public InvoiceServices(FGCDbContext context, HttpClient httpClient, IWebHostEnvironment _Environment, IConverter converter)
        {
            this.context = context;
            Environment = _Environment;
            _httpClient = httpClient;
            _emailService = new EmailService();
            _converter = converter;
            rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/Documents/");
        }

        public Microsoft.AspNetCore.Http.HttpContext GetHttpContext()
        {
            return HttpContext;
        }


        [HttpGet]
        public async Task<InvoicesVMForTable> GetInvoices(string JobId)
        {
            InvoicesVMForTable lstInvoices = new InvoicesVMForTable();
            try
            {
                var records = SFConnect.client.Query<Invoice>($"SELECT Id, Invoice_Title__c,Invoice_Date__c,Invoice_Email_To__c,Status__c FROM NDIS_Invoice__c  WHERE  NDIS_Job__c = '{JobId}'");
                var queryable = (from c in records
                                 select new InvoicesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Invoice_Title__c,
                                     InvoiceDate=c.Invoice_Date__c,
                                     SentTo=c.Invoice_Email_To__c,
                                     Status=c.Status__c
                                   
                                 });

                lstInvoices.Invoices = queryable.ToList();
                lstInvoices.TotalCount = queryable.Count();
                return lstInvoices;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }


        [HttpGet]
        public async Task<InvoicesVMForTable> GetNDISInvoices([FromQuery] PaginationDTO pagination,string OTId)
        {
            InvoicesVMForTable lstInvoices = new InvoicesVMForTable();
            try
            {
                var records = SFConnect.client.Query<Invoice>("SELECT Id, Invoice_Title__c,Customer_Name__c, Invoice_Date__c, Invoice_Email_To__c, Invoice_Total__c,CreatedDate, Job_Number__c,NDIS_Job__c ,Status__c, OT__c  FROM NDIS_Invoice__c  WHERE  OT__c='" + OTId + "' AND IsDelete__c <> 'Yes' ORDER BY CreatedDate DESC ");
                var queryable = (from c in records
                                 select new InvoicesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Invoice_Title__c.ToString(),
                                     Customer_Name = c.Customer_Name__c,
                                     InvoiceDate = c.Invoice_Date__c.Value,
                                     SentTo = c.Invoice_Email_To__c,
                                     InvoiceTotal = c.Invoice_Total__c,
                                     Job_Number = c.Job_Number__c,
                                     JobID = c.NDIS_Job__c,
                                     OT = c.OT__c,
                                     Status = c.Status__c

                                 }).AsQueryable();
                lstInvoices.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                lstInvoices.Invoices = queryable.Paginate(pagination).ToList();
                lstInvoices.TotalCount = queryable.Count();
               
                return lstInvoices;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }



        [HttpGet]
        public async Task<InvoicesVMForTable> GetNDISInvoices([FromQuery] PaginationDTO pagination)
        {
            InvoicesVMForTable lstInvoices = new InvoicesVMForTable();
            try
            {
                var records = SFConnect.client.Query<Invoice>("SELECT Id, Invoice_Title__c,Customer_Name__c, Invoice_Date__c, Invoice_Email_To__c, Invoice_Total__c,CreatedDate, Job_Number__c,NDIS_Job__c ,Status__c, OT__c  FROM NDIS_Invoice__c  WHERE IsDelete__c <> 'Yes' ORDER BY CreatedDate DESC ");
                var queryable = (from c in records
                                 select new InvoicesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Invoice_Title__c.ToString(),
                                     Customer_Name = c.Customer_Name__c,
                                     InvoiceDate = c.Invoice_Date__c.Value,
                                     SentTo = c.Invoice_Email_To__c,
                                     InvoiceTotal = c.Invoice_Total__c,
                                     Job_Number = c.Job_Number__c,
                                     JobID = c.NDIS_Job__c,
                                     OT = c.OT__c,
                                     Status = c.Status__c

                                 }).AsQueryable();
                lstInvoices.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                lstInvoices.Invoices = queryable.Paginate(pagination).ToList();
                lstInvoices.TotalCount = queryable.Count();

                return lstInvoices;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<InvoicesVMForTable> GetInvoiceSearch([FromQuery] PaginationDTO pagination, SearchFilterVm searchFilter, string OTId)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchFilter.SearchKey))
                {
                    searchFilter.SearchKey = searchFilter.SearchKey.ToLower();
                    searchFilter.SearchKey = searchFilter.SearchKey.Trim();
                }
                InvoicesVMForTable lstInvoices = new InvoicesVMForTable();
                var records = SFConnect.client.Query<Invoice>("SELECT Id, Invoice_Title__c,Customer_Name__c, Invoice_Date__c, Invoice_Email_To__c, Invoice_Total__c,CreatedDate, Job_Number__c,NDIS_Job__c ,Status__c, OT__c  FROM NDIS_Invoice__c  WHERE  OT__c='" + OTId + "' ORDER BY CreatedDate DESC ");
                var queryable = (from c in records
                                 select new InvoicesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Invoice_Title__c.ToString(),
                                     Customer_Name = c.Customer_Name__c,
                                     InvoiceDate = c.Invoice_Date__c.Value,
                                     SentTo = c.Invoice_Email_To__c,
                                     InvoiceTotal = c.Invoice_Total__c,
                                     Job_Number = c.Job_Number__c,
                                     JobID = c.NDIS_Job__c,
                                     OT = c.OT__c,
                                     Status = c.Status__c

                                 }).AsQueryable();

                if (searchFilter.FromDate != null & searchFilter.ToDate != null)
                {
                    queryable = queryable.Where(q => q.InvoiceDate >= searchFilter.FromDate && q.InvoiceDate <= searchFilter.ToDate);
                }
                if (searchFilter.Status != null)
                {
                    queryable = queryable.Where(q => q.Status ==searchFilter.Status);
                }
                if (!string.IsNullOrEmpty(searchFilter.SearchKey))
                {
                    queryable = (from c in queryable
                                 where (string.IsNullOrEmpty(searchFilter.SearchKey)

                                    || (c.Title != null && c.Title.ToLower().Trim().Contains(searchFilter.SearchKey))
                                    || (c.Customer_Name != null && c.Customer_Name.ToLower().Trim().Contains(searchFilter.SearchKey))
                                    || (c.SentTo != null && c.SentTo.ToString().Trim().ToLower().Contains(searchFilter.SearchKey))
                                    || (c.InvoiceTotal != null && c.InvoiceTotal.ToString().Trim().ToLower().Contains(searchFilter.SearchKey))
                                    || (c.Job_Number != null && c.Job_Number.ToString().Trim().ToLower().Contains(searchFilter.SearchKey))
                                    || (c.Status != null && c.Status.ToLower().Trim().Contains(searchFilter.SearchKey))

                                    )
                                 select c).OrderBy(x => x.Title).AsQueryable();
                }
               
                var queryablee = queryable.AsQueryable();

                lstInvoices.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                lstInvoices.Invoices = queryable.Paginate(pagination).ToList();
                lstInvoices.TotalCount = queryable.Count();
             


                return lstInvoices;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        public async Task<InvoicesVM> GetInvoiceByID(string Id)
        {
            try
            {
                var records = SFConnect.client.Query<Invoice>($"SELECT Id, Invoice_Title__c,NDIS_Job__c,Job_Number__c,OT__c, Invoice_Date__c,Due_Date__c,Invoice_Email_To__c,Status__c FROM NDIS_Invoice__c   WHERE  Id = '{Id}'");
                var invoiceItems = SFConnect.client.Query<InvoiceItems>($"SELECT Id, Item__c, Amount__c,Description__c,Qty_Hours__c,Qty_KMs__c,Qty_Minutes__c,Date__c,Tax_Rate__c,Unit_Price__c  FROM NDIS_Invoice_Items__c WHERE NDIS_Invoice__c = '{Id}'");
                var queryable = (from c in records
                                 select new InvoicesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Invoice_Title__c,
                                     InvoiceDate = c.Invoice_Date__c,
                                     SentTo = c.Invoice_Email_To__c,
                                     Status = c.Status__c,
                                     JobID = c.NDIS_Job__c,
                                     Job_Number = c.Job_Number__c,
                                     OT = c.OT__c,
                                     DueDate = c.Due_Date__c,
                                     InvoiceItemList = invoiceItems.Select(ii=> new InvoiceItemsVm
                                     {
                                         //Amount = ii.Amount__c,
                                         Date = ii.Date__c,
                                         ItemName = ii.Item__c,
                                         Description = ii.Description__c,
                                         Hours = ii.Qty_Hours__c,
                                         Minutes = ii.Qty_Minutes__c,
                                         KM = ii.Qty_KMs__c,
                                         UnitPrice = ii.Unit_Price__c,
                                         Tax = ii.Tax_Rate__c.ToString()
                                     }).ToList(),
                                 }).FirstOrDefault();

                return queryable;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }
        
        public async Task<InvoicesVM> GetInvoiceItems(string Id)
        {
            try
            {
                var records = SFConnect.client.Query<Invoice>($"SELECT Id,Invoice_Title__c, NDIS_Job__c,OT__c,Filename__c,Invoice_Date__c,Due_Date__c, Invoice_Filepath__c, Invoice_Email_To__c,Status__c, Inv_Number__c, OT__r.FullName__c, OT__r.AHPRA_No__c,OT__r.ABN__c, OT__r.Bank_Account_BSB__c, OT__r.Bank_Account_Number__c, OT__r.Email__c,  NDIS_Job__r.NDIS_Plan_Number__c FROM NDIS_Invoice__c   WHERE  Id = '{Id}'");
                var invoiceItems = SFConnect.client.Query<InvoiceItems>($"SELECT Id, Item__c, Amount__c,Description__c,Rate__c,Qty_Hours__c,Qty_KMs__c,Qty_Minutes__c,Date__c,Tax_Rate__c,Unit_Price__c  FROM NDIS_Invoice_Items__c WHERE NDIS_Invoice__c = '{Id}'");
                var queryable = (from c in records
                                 select new InvoicesVM
                                 {
                                     Id = c.Id,
                                     SentTo = c.Invoice_Email_To__c,
                                     Status = c.Status__c,
                                     JobID = c.NDIS_Job__c,
                                     Title = c.Invoice_Title__c,
                                     Filename = c.Filename__c,
                                     Folder = c.Invoice_Filepath__c,
                                     DueDate = c.Due_Date__c,
                                     InvoiceNo=c.Inv_Number__c,
                                     OT = c.OT__c,
                                     NDISNumber= c.NDIS_Plan_Number__c,
                                     AHPRANo=c.AHPRA_No__c,
                                     ABN=c.ABN__c,
                                     OTName=c.FullName__c,
                                     OTEmail=c.Email__c,
                                     Bank_Account_BSB__c=c.Bank_Account_BSB__c,
                                     Bank_Account_Number__c=c.Bank_Account_Number__c,                                     
                                     InvoiceItemList = invoiceItems.Select(ii => new InvoiceItemsVm
                                     {
                                         
                                         Date = ii.Date__c,
                                         ItemName = ii.Item__c,
                                         Description = ii.Description__c,
                                         Hours = ii.Qty_Hours__c,
                                         Minutes = ii.Qty_Minutes__c,
                                         KM = ii.Qty_KMs__c,
                                         NetAmount = ii.Amount__c,
                                         UnitPrice = ii.Unit_Price__c,
                                         Rate = ii.Rate__c,
                                         Tax = ii.Tax_Rate__c.ToString()
                                     }).ToList(),
                                 }).FirstOrDefault();
                

                var OTInfo = SFConnect.client.Query<OT>($"SELECT  FullName__c, AHPRA_No__c, ABN__c, Bank_Account_BSB__c, Bank_Account_Number__c, Email__c  FROM OTs__c   WHERE  Id = '{queryable.OT}'");
                queryable.OTName = OTInfo.FirstOrDefault().Fullname__c;
                queryable.OTEmail = OTInfo.FirstOrDefault().Email__c;
                queryable.Bank_Account_BSB__c = OTInfo.FirstOrDefault().Bank_Account_BSB__c;
                queryable.Bank_Account_Number__c = OTInfo.FirstOrDefault().Bank_Account_Number__c;
                queryable.ABN = OTInfo.FirstOrDefault().ABN__c;

                var JobInfo = SFConnect.client.Query<NDIS>($"SELECT NDIS_Plan_Number__c FROM NDIS_Job__c   WHERE  Id = '{queryable.JobID}'");
                queryable.NDISNumber=JobInfo.FirstOrDefault().NDIS_Plan_Number__c.ToString();

                return queryable;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<List<SelectListItem>> GetAllOTs()
        {
            List<SelectListItem> ots = new List<SelectListItem>();

            try
            {
                
                var records = SFConnect.client.Query<OT>("SELECT Id, Fullname__c FROM OTs__c");

                ots = records.Select(r => new SelectListItem
                {
                    Text = r.Fullname__c,
                    Value = r.Id
                }).ToList();

            }
            catch (SalesforceException ex)
            {

            }
            return ots;

        }


        [HttpPost]
        public async Task<string> AddInvoice(InvoicesVM Model, string JobId,string htmlcontent)
        {
            try
            {
                string result = SFConnect.client.Create("NDIS_Invoice__c",
                  new
                  {
                      Invoice_Title__c=Model.Title,
                      Invoice_Date__c= Convert.ToDateTime(Model.InvoiceDate.Value.ToString("yyyy-MM-ddTHH:mm:ssK")),
                      Invoice_Email_To__c=Model.SentTo,
                      Status__c=Model.Status,
                      NDIS_Job__c = JobId,
                      Job_Number__c=Model.Job_Number,
                      Customer_Name__c=Model.Customer_Name,
                      Invoice_Filepath__c=Model.Folder,
                      Filename__c=Model.Filename,
                      Due_Date__c= Convert.ToDateTime(Model.DueDate.Value.ToString("yyyy-MM-ddTHH:mm:ssK")),
                      OT__c =Model.OT

                  });
              await CreatePDFAsync(htmlcontent, Model.Filename);

                // Save the invoice in attachments
                // Save Attachment to Salesforce

                var filepath = Path.Combine(Environment.WebRootPath, "Documents/" + Model.Folder+"/" + Model.Filename);
                byte[] data =System.IO.File.ReadAllBytes(filepath);
                string fileID = SFConnect.client.Create("ContentVersion",
                       new
                       {
                           Title = Model.Filename,
                           PathOnClient = filepath,
                           VersionData = data,
                           Origin = "H",
                           FirstPublishLocationId = result


                       });
                return result;

            }
            catch (SalesforceException ex)
            {
                return "ERORR:" + ex.Message;
              
            }

        }

        public List<SelectListItem> GetTaxesList()
        {
            //IQueryable<SelectListItem> DropdownList;
            var records = SFConnect.client.Query<Tax>($"SELECT Id, Name,Value__c  FROM Tax__c ORDER BY Name");
            var DropdownList = (from c in records
                                select new SelectListItem
                                {
                                    Text = c.Name,
                                    Value = c.Value__c.ToString(),

                                });
            
            return DropdownList.ToList();
        }
        public async Task<string> UpdateInvoice(InvoicesVM Model, string JobId, string htmlcontent)
        {
            try
            {
                SFConnect.client.Update("NDIS_Invoice__c", Model.Id, 
                  new
                  {
                      Invoice_Title__c = Model.Title,
                      Invoice_Date__c = Convert.ToDateTime(Model.InvoiceDate.Value.ToString("yyyy-MM-ddTHH:mm:ssK")),
                      Invoice_Email_To__c = Model.SentTo,
                      Status__c = Model.Status,
                      NDIS_Job__c = JobId,
                      Job_Number__c = Model.Job_Number,
                      Customer_Name__c = Model.Customer_Name,
                      Invoice_Filepath__c = Model.Folder,
                      Filename__c = Model.Filename,
                      Due_Date__c = Convert.ToDateTime(Model.DueDate.Value.ToString("yyyy-MM-ddTHH:mm:ssK")),
                      OT__c = Model.OT

                  });
                await CreatePDFAsync(htmlcontent, Model.Filename);

                // Save the invoice in attachments
                // Save Attachment to Salesforce

                var filepath = Path.Combine(Environment.WebRootPath, "Documents/" + Model.Folder + "/" + Model.Filename);
                byte[] data = System.IO.File.ReadAllBytes(filepath);
                string fileID = SFConnect.client.Create("ContentVersion",
                       new
                       {
                           Title = Model.Filename,
                           PathOnClient = filepath,
                           VersionData = data,
                           Origin = "H",
                           FirstPublishLocationId = Model.Id


                       });
                return Model.Id;

            }
            catch (SalesforceException ex)
            {
                return "ERORR:" + ex.Message;

            }
        }


        public async Task DeleteInvoice(string id)
        {
            try
            {
                SFConnect.client.Update("NDIS_Invoice__c", id,
                  new
                  {
                      IsDelete__c = "Yes"
                  });
                

                // Save the invoice in attachments
                // Save Attachment to Salesforce

            }
            catch (SalesforceException ex)
            {
                throw;

            }
        }
        public List<SelectListItem> GetRateList()
        {
            //IQueryable<SelectListItem> DropdownList;
            var records = SFConnect.client.Query<Rate>($"SELECT Id, Name,Rate_Value__c  FROM Rates__c ORDER BY Name");
            var DropdownList = (from c in records
                                select new SelectListItem
                                {
                                    Text = c.Name + " (" + c.Rate_Value__c + ")",
                                    Value = c.Rate_Value__c.ToString(),

                                });

            return DropdownList.ToList();
        }
        public async Task<Exception> SendInvoiceInEmail(string htmlContent, string filename, InvoicesVM InvoiceModal)
        {
           
            await CreatePDFAsync(htmlContent, filename);
            string subject = "Invoice | Ardant | " + InvoiceModal.OTName + " | " + InvoiceModal.NDISNumber;
            string filepath = Path.Combine(rootFolder, InvoiceModal.Folder,InvoiceModal.Filename);
            //  Send Email
            if (InvoiceModal.SentTo != null)
                    {
                      
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Dear " + InvoiceModal.Customer_Name + "! <br/> <br />");
                        sb.AppendLine("I hope this email finds you well. I am pleased to provide you with  invoice  " + InvoiceModal.InvoiceNo + " for the services delivered to  " + InvoiceModal.Customer_Name + ",  client NDIS number : " + InvoiceModal.NDISNumber  + ". The attached PDF contains service delivery details.  <br/> <br/>");
                sb.AppendLine("The amount outstanding of AUD " + InvoiceModal.Total + "  is due on " + InvoiceModal.DueDate.Value.ToString("dd-MMM-yyyy") + ". <br /> <br/>");
          
                sb.AppendLine("If you have any questions or require further details, please do not hesitate to contact us. We value your business and look forward to your prompt settlement of this invoice.  <br/><br/>");
               
                sb.AppendLine("Thanks and  regards, <br/>Atif Majeed, <br /> Business Manager <br /> MBCAM Pty Ltd. <br /> 0469 224 870");
                try
                {
                    await _emailService.SendEmailAsync(InvoiceModal.SentTo, subject, sb.ToString(), filepath);
                    return new Ok("1");

                }
                catch (Exception ex)
                {
                    return new BadRequestException(ex.Message.ToString());

                }

               
            }

            return null;      



       }

               
        public async Task<IActionResult> CreatePDFAsync(string htmlContent, string filename)
        {

            string path = Path.Combine(rootFolder, DateTime.Now.Year.ToString()+"_documents", filename);
            GlobalSettings globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = DinkToPdf.Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 10 },
                DocumentTitle = "NDIS Invoice_" + DateTime.Now.ToString("yyyymmddhhmmss"),
                Out = path  //USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };

            ObjectSettings objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                //Page = "~/PdfTemplate/ViewPdf/?ApplicationId=" + ApplicationId, //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
                WebSettings = { DefaultEncoding = "utf-8", MinimumFontSize = 12 },
                //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = false },
                FooterSettings = { FontName = "Lucida Sans", FontSize = 9, Right = "Page [page]" }
            };
            HtmlToPdfDocument pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            //_converter.Convert(pdf); IF WE USE Out PROPERTY IN THE GlobalSettings CLASS, THIS IS ENOUGH FOR CONVERSION
            byte[] file = _converter.Convert(pdf);
            //return Ok("Successfully created PDF document.");
            // return File(file, "application/pdf", "FxTransaction.pdf");
            return File(file, "application/pdf");

        }
        [HttpPost]
        public async Task<Exception> UpdateInvoice(InvoicesVM Model, string Id)
        {
            try
            {
                SFConnect.client.Update("NDIS_Invoice__c", Id, new
                {
                    Invoice_Title__c = Model.Title,
                    Invoice_Date__c = Model.InvoiceDate,
                    Invoice_Email_To__c = Model.SentTo,
                    Status__c = Model.Status,
                });
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }
        //public async Task<Exception> DeleteInvoice(string Id)
        //{
        //    try
        //    {
        //        SFConnect.client.Delete("NDIS_Invoice__c", Id);
        //        return new Ok("1");
        //    }
        //    catch (SalesforceException ex)
        //    {
        //        return new BadRequestException(ex.Message.ToString());
        //    }
        //}

        public async Task<string> GetInvoiceHtmlContent(string baseUrl,string invoiceId)
        {

            var response = await _httpClient.GetAsync($"{baseUrl}/api/pdfcreator/CreatePDFAsyncNew/?InvoiceId={invoiceId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "Failed to load HTML content";
            }
        }

        public byte[] CreateReconciliationSheet(List<InvoicesVM> lstExport)
        {
            XLWorkbook workbook = new();
            workbook.Properties.Title = "Invoice";
            workbook.Properties.Author = "Sami-Ardant";
            workbook.Properties.Subject = "";
            workbook.Properties.Keywords = "";
            CreateInvoiceDownloadSheet(workbook, lstExport);
            return ConvertToByte(workbook);
        }

        private void CreateInvoiceDownloadSheet(XLWorkbook package, List<InvoicesVM> lstExport)
        {
            IXLWorksheet worksheet = package.Worksheets.Add("Invoices");
            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "Job Number";
            worksheet.Cell(1, 3).Value = "Customer Name";
            worksheet.Cell(1, 4).Value = "Invoice Title";
            worksheet.Cell(1, 5).Value = "Sent To";
            worksheet.Cell(1, 6).Value = "Invoice Total";
            worksheet.Cell(1, 7).Value = "Status";
           
            worksheet.ExpandColumns();
            int index = 1;
            foreach (var item in lstExport)
            {
                worksheet.Cell(index + 1, 1).Value = item.InvoiceDate;
                worksheet.Cell(index + 1, 2).Value = item.Job_Number;
                worksheet.Cell(index + 1, 3).Value = item.Customer_Name;
                worksheet.Cell(index + 1, 4).Value = item.Title;
                worksheet.Cell(index + 1, 5).Value = item.SentTo;
                worksheet.Cell(index + 1, 6).Value = item.InvoiceTotal;
                worksheet.Cell(index + 1, 7).Value = item.Status;

                index++;
            }

        }

        private byte[] ConvertToByte(XLWorkbook workbook)
        {
            MemoryStream stream = new();
            workbook.SaveAs(stream);
            byte[] content = stream.ToArray();
            return content;
        }


    }
}
