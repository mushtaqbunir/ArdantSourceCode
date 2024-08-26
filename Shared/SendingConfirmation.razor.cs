using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class SendingConfirmation
    {
        [Parameter]
        public bool showModal { get; set; }

        [Parameter]
        public bool IsFile { get; set; }

        [Parameter]
        public string Message { get; set; }

        [Parameter]
        public string Title { get; set; }




        [Parameter]
        public EventCallback<bool> OnDeleteVisibilityChangedModel { get; set; }
        [Parameter]
        public EventCallback<bool> OnDeleteAddSuccessModel { get; set; }

        void ModalShow()
        {
            showModal = true;
        }

        public Task ModalCancel()
        {
            showModal = false;
            return OnDeleteVisibilityChangedModel.InvokeAsync(false);
        }
        public Task ModalOk()
        {
            Console.WriteLine("Modal ok");
            showModal = false;
            //  OnDeleteVisibilityChangedModel.InvokeAsync(true);
            return OnDeleteAddSuccessModel.InvokeAsync(true);
        }
    }
}
