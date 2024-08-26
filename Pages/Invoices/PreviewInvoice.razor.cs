using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Invoices;
using ArdantOffical.Helpers.Enums;
using ArdantOffical.IService;
using ArdantOffical.Services;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArdantOffical.Pages.Invoices
{
    public partial class PreviewInvoice
    {

        [Inject]
        IPdfConverterService PdfConverterService { get; set; }
        [Parameter]
        public EventCallback VisibilityHide { get; set; }
        [Parameter]
        public string HtmlContent { get; set; }
        
        [Parameter]
        public InvoicesVM InvoiceModal { get; set; }
        [Parameter]
        public EventCallback<bool> OnAddSuccess { get; set; }
        public bool IsSpinner { get; set; }
        public bool IsSpinnerDownload { get; set; }
        public TostModel TostModelclass { get; set; } = new();
        public async Task CloseSideBar()
        {
            await VisibilityHide.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            if (InvoiceModal==null)
            {
                InvoiceModal = new();
            }
        }
      
        public async Task DownloadInvoice()
        {
            IsSpinnerDownload = true;
            await Task.Delay(10);

            var pdfBytes = PdfConverterService.ConvertHtmlToPdf(HtmlContent);
            // Convert PDF byte array to base64 string
            var base64String = Convert.ToBase64String(pdfBytes);

            // Invoke JavaScript function to download the PDF
            await js.InvokeVoidAsync("downloadPDF", base64String);
            IsSpinnerDownload = false;
        }
        public async Task SendEmail()
        {
            IsSpinner = true;
           await Task.Delay(10);
            Exception registerResponse = await IinvoiceServices.SendInvoiceInEmail(HtmlContent, InvoiceModal.Filename, InvoiceModal);
            if (registerResponse.Message == "1")
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = "Invoice Sent to the customer";
                TostModelclass.Msgstyle = MessageColor.Success;

            }
            else
            {
                TostModelclass.AlertMessageShow = true;
                TostModelclass.AlertMessagebody = registerResponse.Message;
                TostModelclass.Msgstyle = MessageColor.Error;
            }
            IsSpinner = false;
        }

    }
}
