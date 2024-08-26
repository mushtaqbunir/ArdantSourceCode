using ArdantOffical.Data.ModelVm;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ArdantOffical.IService
{
    public interface IAttachmentServices
    {
        public Task<Exception> SaveAttachments(AttachmentsVm attachmentsVm);
        public Task<List<AttachmentsVm>> GetAllAttachments(string jobId);
        public Task DeleteAttachments(int fileId);
    }
}
