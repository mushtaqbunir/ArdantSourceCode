using ArdantOffical.Data;
using ArdantOffical.Helpers.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace ArdantOffical.Shared
{
    public partial class MainLayout : INotifyPropertyChanged
    {

        [Inject] IJSRuntime _jsRuntime { get; set; }


        [Inject]
        protected ILogger<MainLayout> Logger { get; set; }

        [Inject]
        public IServiceScopeFactory serviceScopeFactory { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        public static bool IsAdminPanel { get; set; } = false;

        //private ErrorBoundary errorBoundary;
        //protected override void OnParametersSet()
        //{
        //    errorBoundary?.Recover();
        //}

        public string HunConnectionStatusColour { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask
        {
            get => _authenticationStateTask;
            set
            {
                _authenticationStateTask = value;
                NotifyPropertyChanged();
            }
        }
        private Task<AuthenticationState> _authenticationStateTask;
        public System.Security.Claims.ClaimsPrincipal User { get; set; }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    User = (await AuthenticationStateTask).User;

        //    if (!(User?.Identity?.IsAuthenticated).GetValueOrDefault())
        //    {
        //        NavigationManager.NavigateTo($"identity/account/Login");

        //    }
        //    if (User != null)
        //    {
        //        await InvokeAsync(() => StateHasChanged());
        //    }
        //    await base.OnAfterRenderAsync(firstRender);
        //}

        protected override async Task OnParametersSetAsync()
        {
            User = (await AuthenticationStateTask).User;

            if (!(User?.Identity?.IsAuthenticated).GetValueOrDefault())
            {
                //NavigationManager.NavigateTo($"identity/account/Login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}", true);               
                // CMS Login page
                //string url = Navigator.Uri;
                NavigationManager.NavigateTo($"identity/account/Login");

            }
            if (User != null)
            {
                //if (User.IsInRole("Introducer User") || User.IsInRole("Introducer Admin"))
                //{
                //    NavigationManager.NavigateTo($"/TransactionsSummary");
                //}
                await InvokeAsync(() => StateHasChanged());
            }
            //if(OneTimeCall > 0)
            //{
            //await ClearlocalStorage();
            //}
            await base.OnParametersSetAsync();
        }
        protected async Task ClearlocalStorage()
        {
            string GetUri = Navigator.Uri;
            if (!GetUri.Contains("Audit/Transactions"))
            {
                if (await localStorage.LengthAsync() > 0)
                {
                    await localStorage.ClearAsync();
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                OneTimeCall = 1;

                //await _jsRuntime.InvokeVoidAsync("initializeJs");
                //var GetUri = Navigator.Uri;
                //if (!GetUri.Contains("Audit/Transactions"))
                //{
                //    await localStorage.ClearAsync();
                //    StateHasChanged();
                //}
                //await RequestNotificationSubscriptionAsync();

            }

            await base.OnAfterRenderAsync(firstRender);
            

        }


        public string Error { get; set; }

        public MainLayout()
        {


        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Inject]
        public AuthenticationStateProvider UserauthenticationStateProvider { get; set; }
        ///--- logout timer
        private System.Timers.Timer timerObj;
        public int OneTimeCall { get; set; } = 0;
        protected override async Task OnInitializedAsync()
        {

            // Set the Timer delay.
            timerObj = new System.Timers.Timer(900_000); // 15 minutes
            timerObj.Elapsed += UpdateTimer;
            timerObj.AutoReset = false;
            // Identify whether the user is active or inactive using onmousemove and onkeypress in JS function.
            //await _jsRuntime.InvokeVoidAsync("timeOutCall", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public void TimerInterval()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Activity Reset -> {DateTime.Now}");
            Console.ResetColor();
            // Resetting the Timer if the user in active state.
            timerObj.Stop();
            // Call the TimeInterval to logout when the user is inactive.
            timerObj.Start();
        }

        private void UpdateTimer(Object source, ElapsedEventArgs e)
        {
            InvokeAsync(async () =>
            {
                // Log out when the user is inactive.
                AuthenticationState authstate = await AuthenticationStateTask;
                //if (authstate.User.Identity.IsAuthenticated)
                //{
                //    NavigationManager.NavigateTo("identity/account/logout", true);
                //}
            });
        }
        public bool ChangePasswordSideBarVisibility { get; set; } = false;
        public void OnPasswordVisibiltyChanged(bool visibilityStatus)
        {
            ChangePasswordSideBarVisibility = visibilityStatus;
        }
        public int UserID { get; set; }
        public async Task ShowChangePasswordSideBar(int id)
        {
            CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
            if (Userinfo != null)
            {
                UserID = Userinfo.UserId;
            }
            OnPasswordVisibiltyChanged(true);
        }
        public async void OnPasswordChangedSuccess()
        {
            //await LoadRecords();
        }
    }
}
