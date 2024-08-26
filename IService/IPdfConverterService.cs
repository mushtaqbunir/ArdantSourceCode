namespace ArdantOffical.IService
{
    public interface IPdfConverterService
    {
        byte[] ConvertHtmlToPdf(string htmlContent);
    }
}
