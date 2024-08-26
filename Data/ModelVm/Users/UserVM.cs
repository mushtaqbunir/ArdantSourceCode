using ArdantOffical.Data.ModelVm.OT;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.Users
{
    public class UserVM
    {

        public int UserID { get; set; }
        public string UserKey { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string SalesforceId { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "User Role is required")]
        public string UserRole { get; set; }
        public int RoleID { get; set; }
        public int? UserStatus { get; set; } = 1;
        public string OnlineStatus { get; set; } = "Offline";
        public string RoleName { get; set; }
        public bool EnableTwoFactor { get; set; }
        public string UserStatusForShow { get { return UserStatus == 1 ? "Active" : "Blocked"; } }

        public bool SkipAuthenticator { get; set; }
        //public int NoOfTxns { get { return FlagTxnCount + NonFlagTxnCount; } }
    }

    public class UserVmUseForTable
    {
        public List<UserVM> UserVmList { get; set; }
        public List<UserVM> User_Export { get; set; }
        public List <OTVm> OTList { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
    public class RolesVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }
        public string MeaningfulName { get; set; }
        public List<ListOfMenuItemFormModel> ListOfRoleClaims { get; set; }
    }
    public class RolesVmUseForTable
    {
        public List<RolesVM> RolesVMVmList { get; set; }

        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
    public class GenericVMUseForTable<T>
    {
        public List<T> ListOfData { get; set; }
        public List<T> AllRecords { get; set; }
        public List<T> SearchRecords { get; set; }
        public int TotalPages { get; set; } = 0;
        public int TotalCount { get; set; } = 0;
    }
}
