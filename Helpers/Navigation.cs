using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers
{
    public static class Navigation
    {
        public static async Task GoBack(IJSRuntime jSRuntime)
        {
            await jSRuntime.InvokeVoidAsync("history.back");
        }
    }
}
