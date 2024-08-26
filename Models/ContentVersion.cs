namespace ArdantOffical.Models
{
    public class ContentVersion
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ContentDocumentId { get; set; }
        public string PathOnClient { get; set; }
        public byte[] VersionData { get; set; }
        public string Origin { get; set; }

    }

}
