using System;
using System.Collections.Generic;

#nullable disable

namespace ArdantOffical.Models
{
    public partial class TblUser
    {
        public int UserId { get; set; }
        public string UserKey { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ShortName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public string Avatar { get; set; }
        public int? UserStatus { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Designation { get; set; }
        public string PasswordReset { get; set; }
        public string OnlineStatus { get; set; }
        public bool EnableTwoFactor { get; set; }
        public DateTime? DateModified { get; set; }
        public ICollection<UserClaim> UserClaims { get; set; }
        public bool IsDelete { get; set; }
        public bool SkipAuthenticator { get; set; }

        public string SalesforceID { get; set; }
        public string ImagePath { get; set; }
        public string ImageTitle { get; set; }
        public string Folder { get; set; }
        public string SignaturePath { get; set; }
        public string SignatureTitle { get; set; }

    }
}
