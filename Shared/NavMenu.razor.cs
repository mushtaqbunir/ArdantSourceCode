using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArdantOffical.Shared
{
    public partial class NavMenu
    {
        public List<MenuItem> ListOfMenuItem = new();
        [Inject]
        IUsersServices UsersServices { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        private HubConnection? hubConnection;
        protected override async Task OnInitializedAsync()
        {
            ListOfMenuItem = await UsersServices.GetMenuItemList();
        }
        //private async Task StartChatHubAsync()
        //{
        //    hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/chatHub")).Build();
        //    await hubConnection.StartAsync();
        //    hubConnection.On<string>("ReceiveOnBoardingNotification", async (message) =>
        //    {
        //        applicationsCount = await IBusinesP.GetApplicationsCount();
        //        StateHasChanged();
        //    });
        //}

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeVoidAsync("attachNavLinksEventListeners");
            }
        }
    }
}
