using System;

#nullable disable

namespace ArdantOffical.Models
{
    public partial class ApiCredential
    {
        public int AuthId { get; set; }
        public string AuthKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Ip { get; set; }
        public string Purpose { get; set; }
        public string Company { get; set; }
        public DateTime IssuedDate { get; set; }
        public string Certificate { get; set; }
        public string CertPassword { get; set; }
        public string CertThumbprint { get; set; }
    }
}
