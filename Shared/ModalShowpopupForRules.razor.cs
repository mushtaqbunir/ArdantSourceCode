using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class ModalShowpopupForRules
    {
        [Parameter]
        public bool showModal { get; set; }
        [Parameter]
        public string Message { get; set; }
        [Parameter]
        public string title { get; set; }
        [Parameter]
        public EventCallback<bool> OnVisibilityChangedModel { get; set; }
        [Parameter]
        public EventCallback<bool> OnAddSuccessModel { get; set; }
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
            showModal = false;
            //OnVisibilityChangedModel.InvokeAsync(false);
            return OnVisibilityChangedModel.InvokeAsync(false);
        }
    }
}
