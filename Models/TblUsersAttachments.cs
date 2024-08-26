using ArdantOffical.Helpers.Enums;

namespace ArdantOffical.Models
{
    public class TblUsersAttachments : BaseModel
    {
        public int UserId { get; set; }
        public string SalesforceId { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string FolderName { get; set; }
        public UserFileType FileType { get; set; }
        public bool IsDelete { get; set; }
    }
}
