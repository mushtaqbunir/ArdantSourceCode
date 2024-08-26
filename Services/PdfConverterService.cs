using ArdantOffical.IService;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace ArdantOffical.Services
{
    public class PdfConverterService : IPdfConverterService
    {
        private readonly IConverter _converter;

        public PdfConverterService(IConverter converter)
        {
            _converter = converter;
        }
        public byte[] ConvertHtmlToPdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DPI = 300
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _converter.Convert(pdf);
        }

    }
}

