using System.Collections.Generic;

namespace ArdantOffical.SignalRHub
{
    public static class SystemActiveUsers
    {
        public static List<ActiveListUser> ActiveUsersList { get; set; } = new List<ActiveListUser>();


    }
    public class ActiveListUser
    {
        public int UserId { get; set; }
        public string OTId { get; set; }
    }
}
