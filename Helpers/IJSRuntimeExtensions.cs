using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers
{
    public static class IJSRuntimeExtensions
    {
        public static ValueTask SaveAs(this IJSRuntime js, string fileName, byte[] content)
        {
            return js.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(content));
        }

        public static ValueTask DisplayMessage(this IJSRuntime js, string message)
        {
            return js.InvokeVoidAsync("Swal.fire", message);
        }

        public static ValueTask DisplayMessage(this IJSRuntime js, string title, string message, SweetAlertMessageType sweetAlertMessageType)
        {
            return js.InvokeVoidAsync("Swal.fire", title, message, sweetAlertMessageType.ToString());
        }
        public static ValueTask MyImageUploaderss(this IJSRuntime js)
        {
            return js.InvokeVoidAsync("MyImageUploader");
            // return js.InvokeVoidAsync("gotoreview(event)");
        }
        public static ValueTask<bool> Confirm(this IJSRuntime js, string title, string message, SweetAlertMessageType sweetAlertMessageType)
        {
            return js.InvokeAsync<bool>("CustomConfirm", title, message, sweetAlertMessageType.ToString());
        }

        public static ValueTask SetInLocalStorage(this IJSRuntime js, string key, string content)
        {
            return js.InvokeVoidAsync(
                  "localStorage.setItem",
                  key, content
                  );
        }

        public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key)
        {
            return js.InvokeAsync<string>(
                           "localStorage.getItem",
                           key
                           );
        }

        public static ValueTask RemoveItem(this IJSRuntime js, string key)
        {
            return js.InvokeVoidAsync(
                           "localStorage.removeItem",
                           key);
        }

        public static ValueTask ShowToast(this IJSRuntime js)
        {

            return js.InvokeVoidAsync("ShowToast");
            // return js.InvokeVoidAsync("gotoreview(event)");
        }
        public static ValueTask Showtooltipp(this IJSRuntime js)
        {

            return js.InvokeVoidAsync("addTooltips");
            // return js.InvokeVoidAsync("gotoreview(event)");
        }

        public static ValueTask CallCkEditor(this IJSRuntime js)
        {

            return js.InvokeVoidAsync("CallCkEditor");
            // return js.InvokeVoidAsync("gotoreview(event)");
        }


        public static ValueTask CallgotoReview(this IJSRuntime js)
        {
            return js.InvokeVoidAsync("gotoReview");
            // return js.InvokeVoidAsync("gotoreview(event)");
        }
        public static ValueTask GetTransactionByType(this IJSRuntime js, string pType)
        {
            return js.InvokeVoidAsync("GetTransactionByType", pType);
        }
        public static ValueTask ImportBespokeCSV(this IJSRuntime js)
        {
            return js.InvokeVoidAsync("ImportCSV");
        }



        public static ValueTask Wizardnextstep(this IJSRuntime js)
        {

            return js.InvokeVoidAsync("nextstep");
            // return js.InvokeVoidAsync("gotoreview(event)");
        }
        public static ValueTask prevstep(this IJSRuntime js)
        {

            return js.InvokeVoidAsync("prevstep");
            // return js.InvokeVoidAsync("gotoreview(event)");
        }
        //public static ValueTask CallSwalShow(this IJSRuntime js)
        //{
        //    return js.InvokeVoidAsync("SwalShow");
        //    // return js.InvokeVoidAsync("gotoreview(event)");
        //}
        //public static ValueTask CallSwalclosee(this IJSRuntime js)
        //{
        //    return js.InvokeVoidAsync("Swalclose");
        //    // return js.InvokeVoidAsync("gotoreview(event)");
        //}

        public static ValueTask focusInputBlazor(this IJSRuntime js, string id)
        {
            return js.InvokeVoidAsync("focusInput", id);
        }
        public static ValueTask focusInputReferenceBlazor(this IJSRuntime js, ElementReference id)
        {
            return js.InvokeVoidAsync("BlazorFocusElement", id);
        }


        public static ValueTask GetNumber_formatter_comma(this IJSRuntime js, string id)
        {
            return js.InvokeVoidAsync("Number_formatter_comma", id);
        }
        public static ValueTask ShowTestDataTablesAdd33(this IJSRuntime js, string id)
        {
            return js.InvokeVoidAsync("CaricaDataTablessr", id);
        }
        public static ValueTask FocusAsync(this IJSRuntime jsRuntime, ElementReference elementReference)
        {
            return jsRuntime.InvokeVoidAsync("BlazorFocusElement", elementReference);
        }
        //public static ValueTask GetIpAddress(this IJSRuntime jsRuntime, ElementReference elementReference)
        //{
        //    var ipAddress = await js.InvokeAsync<string>("getIpAddress").ConfigureAwait(true);

        //    return jsRuntime.InvokeVoidAsync("BlazorFocusElement", elementReference);
        //}
        public static ValueTask<string> GetIpAddress(this IJSRuntime js)
        {
            return js.InvokeAsync<string>("getIpAddress");
        }
    }
    public enum SweetAlertMessageType
    {
        question, warning, error, success, info
    }



}
