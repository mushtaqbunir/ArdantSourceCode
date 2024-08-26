using System.Threading.Tasks;

namespace ArdantOffical.IService
{
    public interface IRazorRendererHelper
    {
        string RenderPartialToString<TModel>(string partialName, TModel model);
       
    }
}
