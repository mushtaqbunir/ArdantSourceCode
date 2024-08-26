using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class Confirm
    {
        [Parameter]
        public string Message { get; set; }
        [Parameter]
        public bool showModal { get; set; }

        [Parameter]
        public EventCallback<bool> OnUndoVisibilityChangedModel { get; set; }
        [Parameter]
        public EventCallback<bool> OnUndoAddSuccessModel { get; set; }

        void ModalShow()
        {
            showModal = true;
        }

        public Task ModalCancel()
        {
            showModal = false;
            return OnUndoVisibilityChangedModel.InvokeAsync(false);
        }
        public Task ModalOk()
        {
            Console.WriteLine("Modal ok");
            showModal = false;
            // return OnDeleteVisibilityChangedModel.InvokeAsync(true);
            // return OnUndoAddSuccessModel.InvokeAsync(true);
            return null;
        }
    }
}
