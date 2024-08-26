using ArdantOffical.Data.ModelVm;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Components
{
    public partial class ActionByModel
    {
        //[Parameter]
        //public ActionByVm ActionByVm { get; set; } = new();
        [Parameter]
        public EventCallback EventCallback { get; set; }
        private async Task CloseActionByModel()
        {
            await EventCallback.InvokeAsync();
        }
    }
}
