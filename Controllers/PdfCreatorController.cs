using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using RazorLight;
using System.Threading.Tasks;
using ArdantOffical.IService;

namespace PDF_Generator.Controllers
{
    [Route("api/pdfcreator")]
    [ApiController]

    public class PdfCreatorController : ControllerBase
    {
        private IConverter _converter;

        private readonly IRazorLightEngine _razorEngine;
        private readonly IRazorRendererHelper _razorRendererHelper;
        private readonly IInvoiceItemsServices _invoiceItemServices;
        public PdfCreatorController(IConverter converter, IRazorLightEngine razorLightEngine,
           IRazorRendererHelper razorRendererHelper, IInvoiceItemsServices invoiceItemsServices)
        {
            _converter = converter;
            _razorRendererHelper = razorRendererHelper;
            _invoiceItemServices = invoiceItemsServices;
        }

        [HttpGet]
        public async Task<IActionResult> CreatePDFAsync(int ApplicationId)
        {


            string partialName = "/Views/FGCApplication.cshtml";
            //string htmlContent = _razorRendererHelper.RenderPartialToString(partialName, BuisnessInformation);


            //var templatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), $"Views/FGCApplication.cshtml");
            //string template = await _razorEngine.CompileRenderAsync(templatePath, BuisnessInformation);

            GlobalSettings globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 10 },
                DocumentTitle = "FGC Application",
                //Out = @"D:\PDFCreator\Employee_Report.pdf"  USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };

            ObjectSettings objectSettings = new ObjectSettings
            {
                PagesCount = true,
                //HtmlContent = htmlContent,
                //Page = "~/PdfTemplate/ViewPdf/?ApplicationId=" + ApplicationId, //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
                WebSettings = { DefaultEncoding = "utf-8" },
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
            //return File(file, "application/pdf", "EmployeeReport.pdf");
            return File(file, "application/pdf");
        }

        //[HttpGet("CreatePDFAsyncNew")]
        //public async Task<IActionResult> CreatePDFAsyncNew(string InvoiceId)
        //{
        //    var invoiceItems = await _invoiceItemServices.GetInvoiceItems(InvoiceId);
        //    string partialName = "/Views/InvoicePdf.cshtml";
        //    string htmlContent = _razorRendererHelper.RenderPartialToString(partialName, invoiceItems);
        //    //var templatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), $"Views/FGCApplication.cshtml");
        //    //string template = await _razorEngine.CompileRenderAsync(templatePath, BuisnessInformation);
        //    GlobalSettings globalSettings = new GlobalSettings
        //    {
        //        ColorMode = ColorMode.Color,
        //        Orientation = Orientation.Portrait,
        //        PaperSize = PaperKind.A4,
        //        Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 10 },
        //        DocumentTitle = "Ardant Invoice",
        //        //Out = @"D:\PDFCreator\Employee_Report.pdf"  USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
        //    };

        //    ObjectSettings objectSettings = new ObjectSettings
        //    {
        //        PagesCount = true,
        //        HtmlContent = htmlContent,
        //        //Page = "~/PdfTemplate/ViewPdf/?ApplicationId=" + ApplicationId, //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
        //        WebSettings = { DefaultEncoding = "utf-8" },
        //        //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = false },
        //        FooterSettings = { FontName = "Lucida Sans", FontSize = 9, Right = "Page [page]" }
        //    };
        //    HtmlToPdfDocument pdf = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = globalSettings,
        //        Objects = { objectSettings }
        //    };
        //    //_converter.Convert(pdf); IF WE USE Out PROPERTY IN THE GlobalSettings CLASS, THIS IS ENOUGH FOR CONVERSION
        //    byte[] file = _converter.Convert(pdf);
        //    //return Ok("Successfully created PDF document.");
        //    //return File(file, "application/pdf", "EmployeeReport.pdf");
        //    //await _IBusiness.AddUsersLog(User, $"Viewed Pdf Application ({BuisnessInformation.BusinessName})");
        //    return File(file, "application/pdf");
        //}
        [HttpGet("CreatePDFAsyncNew")]
        public async Task<IActionResult> CreatePDFAsyncNew(string InvoiceId)
        {
            var invoiceItems = await _invoiceItemServices.GetInvoiceItems(InvoiceId);
            string partialName = "/Views/InvoicePdf.cshtml";
            string htmlContent = _razorRendererHelper.RenderPartialToString(partialName, invoiceItems);

            return Content(htmlContent, "text/html");
        }
    }
}