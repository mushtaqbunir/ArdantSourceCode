using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class ShowDetailMessage
    {
        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }

        [Parameter]
        public string MessageDetail { get; set; }
        [Parameter]
        public string HeaderTitle { get; set; }
        public async Task CloseShowModel()
        {
            await OnVisibilityChanged.InvokeAsync(false);
            StateHasChanged();
        }
    }
}
