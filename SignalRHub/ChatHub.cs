using ArdantOffical.IService;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArdantOffical.SignalRHub
{

    public class ChatHub : Hub
    {
        private static int Count = 0;
        //public static List<SystemActiveUsers> ActiveList;
        IUsersServices iusersService;


        private readonly AuthenticationStateProvider UserauthenticationStateProvider;
        // private readonly AuthenticationStateProvider UserauthenticationStateProvider;

        public IHttpContextAccessor httpContextAccessor;
        public ChatHub(IUsersServices _iusersService)
        {

            this.iusersService = _iusersService;
        }
        public override async Task OnConnectedAsync()
        {
            // List<Claim> Users = 
            var CurrentUserId = 0;
            // var tt=  System.Web.HttpContext.Current.User.Identity.GetUserId();
            Claim CurrentuserInfo = Context.User.Claims.Where(x => x.Type == ClaimTypes.Sid).FirstOrDefault();
            //CurrentUserInfoVM CurrentuserInfo = new CurrentUserInfoVM();
            //CurrentuserInfo = await iusersService.GetCurrentUserInfo();//await UserauthenticationStateProvider.CurrentUser();//httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Sid).FirstOrDefault();  //ClaimsPrincipal.Current.Identity.Name;

            if (CurrentuserInfo != null)
            {
                CurrentUserId = Convert.ToInt32(CurrentuserInfo.Value);
            }
            //SystemActiveUsers ActiveObj;
            // ActiveList = new List<SystemActiveUsers>();
            if (CurrentUserId != 0)
            {

                var ActiveUser = SystemActiveUsers.ActiveUsersList.Where(x => x.UserId == CurrentUserId).FirstOrDefault();
                if (ActiveUser == null)
                {
                    ActiveListUser ActiveObj = new ActiveListUser();
                    ActiveObj.UserId = CurrentUserId;
                    SystemActiveUsers.ActiveUsersList.Add(ActiveObj);

                }
            }
            await base.OnConnectedAsync();
            await Clients.All.SendAsync("ReceiveMessage", SystemActiveUsers.ActiveUsersList);
            await Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {

            int CurrentUserId = 0;
            Claim CurrentuserInfo = Context.User.Claims.Where(x => x.Type == ClaimTypes.Sid).FirstOrDefault();

            if (CurrentuserInfo != null)
            {
                CurrentUserId = Convert.ToInt32(CurrentuserInfo.Value);
            }
            if (CurrentUserId != 0)
            {
                var ACtiveUserId = Convert.ToInt32(CurrentUserId);
                var ActiveUser = SystemActiveUsers.ActiveUsersList.Where(x => x.UserId == ACtiveUserId).FirstOrDefault();
                if (ActiveUser != null)
                {
                    SystemActiveUsers.ActiveUsersList.Remove(ActiveUser);
                }
            }

            base.OnDisconnectedAsync(exception);
            Clients.All.SendAsync("ReceiveMessage", SystemActiveUsers.ActiveUsersList);
            return Task.CompletedTask;
        }
        public async Task SendMessage()
        {
            // ArdantOffical.Pages.Users.MyActiveList(ActiveList);
            await Clients.All.SendAsync("ReceiveMessage", SystemActiveUsers.ActiveUsersList);

        }

        public async Task SendMessageTranscationCounter(int Type)
        {
            await Clients.All.SendAsync("ReceiveTranscationCounter", Type);
        }
        public async Task SendOnBoardingNotification()
        {
            await Clients.All.SendAsync("ReceiveOnBoardingNotification","");
        }
        public async Task SendFIUEscalationNotification()
        {
            await Clients.All.SendAsync("ReceiveFIUEscalationNotification", "");
        }
        public async Task SendObservationQueueNotifications(int type)
        {
            await Clients.All.SendAsync("ObservationQueueNotifications", type);
        }
    }
}
