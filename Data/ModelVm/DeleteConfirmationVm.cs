namespace ArdantOffical.Data.ModelVm
{
    public class DeleteConfirmationVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DeleteType { get; set; }
        public int Crid { get; set; }
        public bool IsFile { set; get; }

        public string Message { get; set; }
    }
}
