using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.SystemLog;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArdantOffical.IService
{
    public interface ILogServices
    {
        void SaveSystemLog(SystemLogVM log);
        //Task<SystemLogPagination> SystemLogDownload([FromQuery] PaginationDTO pagination, SearchParams searchParams, string SearchFilter = "");
        //Task<List<SystemLogVM>> SystemLogDownloadExcel(SearchParams searchParams);
    }
}
