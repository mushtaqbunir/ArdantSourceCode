using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class CancelConfirmation
    {
        [Parameter]
        public bool showModal { get; set; }

        [Parameter]
        public string Message { get; set; }

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
          
            return OnDeleteAddSuccessModel.InvokeAsync(true);
        }
    }
}
