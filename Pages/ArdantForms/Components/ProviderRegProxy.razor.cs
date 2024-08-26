using Microsoft.AspNetCore.Components;

namespace ArdantOffical.Pages.ArdantForms.Components
{
    public partial class ProviderRegProxy
    {
        [Inject]
        public NavigationManager NavManager { get; set; }
        protected override void OnAfterRender(bool firstRender)
        {
            NavManager.NavigateTo("ProviderRegistrationForm");
        }
    }
}
