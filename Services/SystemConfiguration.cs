using ArdantOffical.Helpers;
using ArdantOffical.IService;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArdantOffical.Services
{
    public class SystemConfiguration : PageModel, ISystemConfiguration
    {
        public IJSRuntime JsR;
        public HttpClient Http;
        public SystemConfiguration(IJSRuntime _JsR, HttpClient _htttclient)
        {
            this.JsR = _JsR;
            this.Http = _htttclient;
        }

        public async Task<string> GetRemoteIpAddress()
        {
            string ipAddress = await JsR.GetIpAddress();
            if (string.IsNullOrEmpty(ipAddress))
            {
                return string.Empty;
            }

            return ipAddress;

        }



    }
}
