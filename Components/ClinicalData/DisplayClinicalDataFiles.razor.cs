using ArdantOffical.Data.ModelVm;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace ArdantOffical.Components.ClinicalData
{
    public partial class DisplayClinicalDataFiles
    {
        [Parameter]
        public bool ModelVisible { get; set; }
        [Parameter]
        public int FileId { get; set; }
        [Parameter]
        public string FileName { get; set; }
        [Parameter]
        public string Folder { get; set; }
        [Parameter]
        public string FileTitle { get; set; }
        [Parameter]
        public EventCallback<AttachmentsVm> DeleteFileVisibilityChanged { get; set; }

        AttachmentsVm attachment = new AttachmentsVm();
        protected override async Task OnInitializedAsync()
        {
            attachment.Path = FileName;
            attachment.ID = FileId;
        }
        public void LocalDeleteFile(AttachmentsVm attachments)
        {

            DeleteFileVisibilityChanged.InvokeAsync(attachments);

        }

    }
}

