using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class ValidationComponent
    {
        [Parameter]
        public bool showModal { get; set; } = true;
        [Parameter]
        public string Message { get; set; }
        [Parameter]
        public string title { get; set; }
        [Parameter]
        public int BPID { get; set; }

        [Parameter]
        public string ButtonText { get; set; } = "ok";
        
        [Parameter]
        public EventCallback<bool> OnVisibilityChangedModel { get; set; }
        [Parameter]
        public EventCallback<bool> OnDeleteSuccess { get; set; }

        void ModalShow()
        {
            showModal = true;
        }

        public Task ModalCancel()
        {
            showModal = false;
            return OnVisibilityChangedModel.InvokeAsync(false);
        }
        public Task ModalOk()
        {
            Console.WriteLine("Modal ok");
            //Task<Exception> registerResponse = _IBespokeMontioringobj.DeleteBespoke(BPID);
            showModal = false;
            OnDeleteSuccess.InvokeAsync(true);
            return OnVisibilityChangedModel.InvokeAsync(true);

        }
    }
}
