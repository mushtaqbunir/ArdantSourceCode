namespace ArdantOffical.Models
{
    public class AppAuthkey
    {
        public int AuthID { get; set; }
        public string AuthKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string IP { get; set; }
        public string Purpose { get; set; }

        public System.DateTime IssuedDate { get; set; }
        public string Certificate { get; set; }
        //public string AiuUserId { get; set; }
    }
}
