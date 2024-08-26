using ArdantOffical.Data.ModelVm;
using ArdantOffical.Helpers.Enums;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class FgcToast
    {
        [Parameter]
        public TostModel TostModels { get; set; }
        //[Parameter]
        //public bool alertMessageShow { get; set; } = false;
        //[Parameter]
        //public string title { get; set; } = "";
        //[Parameter]
        //public string msgstyleClass { get; set; } = "";
        //[Parameter]
        //public string Message { get; set; }
        public string CssStyles { get; set; }
       
        protected override async Task OnParametersSetAsync()
        {
            if (TostModels.AlertMessageShow)
            {
                if (TostModels.Msgstyle == MessageColor.Success)
                {
                    CssStyles = "opacity: 1;margin-right:-37px; width: 400px;margin-top:0px;    position: fixed";
                }
                else
                {
                    CssStyles = "opacity: 1;right:0px; width: 400px;margin-top:19px;    position: fixed;height: 45px";
                }
                await ShowMessagePopup();
            }
        }

        private async Task ShowMessagePopup()
        {
            // Wait 5 seconds for the user to read the message, and then close the modal
            await Task.Delay(5000);
            TostModels.AlertMessageShow = false;
            //await InvokeAsync(() => StateHasChanged());
        }
        public void Close()
        {
            TostModels.AlertMessageShow = false;
        }
    }
}
