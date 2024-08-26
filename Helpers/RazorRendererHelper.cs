using ArdantOffical.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers
{
    public class RazorRendererHelper : IRazorRendererHelper
    {
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;

        public RazorRendererHelper(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public string RenderPartialToString<TModel>(string partialName, TModel model)
        {
            ActionContext actionContext = GetActionContext();
            IView partial = FindView(actionContext, partialName);

            using (StringWriter output = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(
                    actionContext,
                    partial,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions()
                );

                partial.RenderAsync(viewContext).ConfigureAwait(false);
                return output.ToString();
            }
        }
      
        private IView FindView(ActionContext actionContext, string partialName)
        {
            ViewEngineResult getPartialResult = _viewEngine.GetView(null, partialName, false);
            if (getPartialResult.Success)
            {
                return getPartialResult.View;
            }

            ViewEngineResult findPartialResult = _viewEngine.FindView(actionContext, partialName, false);
            if (findPartialResult.Success)
            {
                return findPartialResult.View;
            }

            System.Collections.Generic.IEnumerable<string> searchedLocations = getPartialResult.SearchedLocations.Concat(findPartialResult.SearchedLocations);
            string errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations)); ;
            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
