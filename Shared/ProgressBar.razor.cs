using Microsoft.AspNetCore.Components;

namespace ArdantOffical.Shared
{
    public partial class ProgressBar
    {
        [Parameter]
        public double progressPercentage { get; set; }
        [Parameter]
        public string progressBarDisplay { get; set; }
    }
}
