using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.SystemLog;
using ArdantOffical.Helpers;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Pages
{
    public partial class SystemLog
    {
        private int currentPage = 1;
        private int totalPageQuantity;
        private int totalCount = -1;
        public PaginationDTO paginationObj { get; set; }
        public SystemLogPagination ListOfRecord { get; set; }
        public List<SystemLogVM> PerPageSystemLog { get; set; }
        public List<SystemLogVM> SystemLog_All { get; set; }
        public List<SystemLogVM> SearchLog_All { get; set; }
        public IQueryable<SystemLogVM> SearchLog_AllFilter { get; set; }
        public SearchParamsVm SystemLogVM = new();
        public List<SelectListItem> lstUsers = new();

        public bool IsloaderShow { get; set; } = false;
        /*   Modal Popup Params */
        public bool showModal { get; set; } = false;
        public string Message { get; set; }
        public string title { get; set; }
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }

        // Download File name
        private string fileName = "Download" + DateTime.Now.ToString("ddMMyyyyhhmmss");
        public bool ModalShowpopupVisibility { get; set; } = false;
        public Task OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;
            return OnVisibilityChanged.InvokeAsync(false);
        }

        private async Task SelectedPage(int page)
        {
            currentPage = page;
            await LoadRecords(page, paginationObj.QuantityPerPage);
        }
        [Inject]
        public FGCDbContext Context { get; set; }
        private List<SelectListItem> GetUsers()
        {
            List<SelectListItem> userList = (from user in Context.TblUsers.AsEnumerable()
                                             select new SelectListItem
                                             {
                                                 Text = user.Firstname + " " + user.Lastname,
                                                 Value = user.UserId.ToString()
                                             }).ToList();
            //Add Default Item at First Position.
            userList.Insert(0, new SelectListItem { Text = "--Select User--", Value = "0" });
            userList.Insert(1, new SelectListItem { Text = "All", Value = "1111" });
            return userList;

        }

        async Task LoadRecords(int page = 1, int quantityPerPage = 25)
        {
            currentPage = page;
            paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
            //ListOfRecord = await IlogServices.SystemLogDownload(paginationObj, SystemLogVM);
            if (ListOfRecord != null)
            {
                PerPageSystemLog = ListOfRecord.SystemLogPerPage;
                SystemLog_All = ListOfRecord.SystemLogAll_Export;
                SearchLog_All = ListOfRecord.SystemLogPerPageVmListForsearch;
                totalPageQuantity = ListOfRecord.TotalPages;
                totalCount = ListOfRecord.TotalCount;
            }
        }
        public List<SelectListItem> ListOfTablePages = new List<SelectListItem>();
        protected override async Task OnInitializedAsync()
        {
            lstUsers = GetUsers();
            //SystemLogVM.TransactionStatus = "-";
           // SystemLogVM.TransactionType = "-";
            ListOfTablePages.Add(new SelectListItem() { Text = "50", Value = "50" });
            ListOfTablePages.Add(new SelectListItem() { Text = "75", Value = "75" });
            ListOfTablePages.Add(new SelectListItem() { Text = "100", Value = "100" });
            try
            {
                await LoadRecords();

            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
            }

        }
        /// <summary>
        /// Download CSV File
        /// </summary>
        [Inject]
        public ILogServices LogServices { get; set; }
        //private async void ExportAsCSV()
        //{
        //    //SystemLog_All = await IlogServices.SystemLogDownloadExcel(SystemLogVM);
        //   // IexportService.SetSystemLog(SystemLog_All);
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        using (StreamWriter writer = new StreamWriter(memoryStream))
        //        {
        //            using (CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(writer))
        //            {
        //                csv.WriteRecords(SystemLog_All);
        //            }
        //            var arr = memoryStream.ToArray();
        //            await js.SaveAs("SystemLog.csv", arr);
        //        }
        //    }
        //    #region Save Log
        //    var log = new SystemLogVM();
        //    log.Object = "System Configuration (System Logs)";
        //    log.Action = "Downloaded";
        //    log.Remarks = "System Logs CSV Downloaded";
        //    LogServices.SaveSystemLog(log);
        //    #endregion
        //}
        //public void DownloadCSVAsync()
        //{
        //    try
        //    {
        //        ExportAsCSV();
        //    }
        //    catch (Exception ex)
        //    {
        //        responseHeader = "ERROR";
        //        responseBody = ex.Message;
        //        responseDialogVisibility = true;
        //    }
        //}
        public int IsDownloadStarted { get; set; } = 0;

        public async Task DownloadExcelAsync()
        {
            try
            {
                IsDownloadStarted = 1;
               // SystemLog_All = await IlogServices.SystemLogDownloadExcel(SystemLogVM);
               // IexportService.SetSystemLog(SystemLog_All);
                //byte[] response = IexportService.CreateSystemLogExcel();
                fileName += ".xlsx";
                // Invoke Js function saveAsFile to save the response as Excel file
              //  await js.InvokeAsync<object>("saveAsFile", fileName, Convert.ToBase64String(response));
                IsDownloadStarted = 2;
                #region Save Log
                var log = new SystemLogVM();
                log.Object = "System Configuration (System Logs)";
                log.Action = "Downloaded";
                log.Remarks = "System Logs Excel Downloaded";
                LogServices.SaveSystemLog(log);
                #endregion
                StateHasChanged();
            }
            catch (Exception ex)
            {
                responseHeader = "ERROR";
                responseBody = ex.Message;
                responseDialogVisibility = true;
            }
        }

        //public async Task SearchSystemLog()
        //{
        //    try
        //    {

        //        IsloaderShow = true;
        //        if (SystemLogVM.StartDate != null)
        //        {
        //            if (SystemLogVM.EndDate >= SystemLogVM.StartDate)
        //            {

        //                if (string.IsNullOrEmpty(SystemLogVM.StartTime.ToString()))
        //                {
        //                    SystemLogVM.StartDate = Convert.ToDateTime(SystemLogVM.StartDate.Value.ToShortDateString().Split(" ")[0]);
        //                    SystemLogVM.EndDate = Convert.ToDateTime(SystemLogVM.EndDate.Value.ToShortDateString().Split(" ")[0]).AddSeconds(86399);

        //                }
        //                else
        //                {
        //                    DateTime st = Convert.ToDateTime(SystemLogVM.StartDate.Value.ToShortDateString() + " " + SystemLogVM.StartTime.ToString());
        //                    SystemLogVM.StartDate = st.DateTime_UK();
        //                    DateTime et = Convert.ToDateTime(SystemLogVM.EndDate.Value.ToShortDateString() + " " + SystemLogVM.EndTime.ToString());
        //                    SystemLogVM.EndDate = et.DateTime_UK();

        //                }

        //                await LoadRecords(1, paginationObj.QuantityPerPage);
        //                IsloaderShow = false;

        //            }
        //            else
        //            {
        //                responseHeader = "Invalid Date Range";
        //                responseBody = "End date must be greater than or equal to Start date";
        //                responseDialogVisibility = true;
        //                IsloaderShow = false;
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        IsloaderShow = false;
        //        responseBody = ex.Message.Contains("SqlDateTime overflow") ? "Please select start and end dates" : ex.Message;
        //        responseHeader = "ERROR";
        //        responseDialogVisibility = true;
        //    }
        //}

        //Action Log 
        public int CrIdAction { get; set; }
        public void ShowAddActionLogSideBar(int id)
        {
            CrIdAction = id;
            OnAddActionLogVisibilityChanged(true);
        }
        public bool AddActionLogSideBarVisibility { get; set; } = false;
        public void OnAddActionLogVisibilityChanged(bool visibilityStatus)
        {
            AddActionLogSideBarVisibility = visibilityStatus;
        }
        public async void OnActionLogSuccess(bool isAdded)
        {
            if (isAdded)
            {
                // await LoadPaymentList();
                //DepartmentList = await Depart.GetDepartments();
                StateHasChanged();

            }
        }
        #region Searching
        public string SearchFilter { get; set; } = "";
        private async Task SearchChanged(string searchFilter)
        {
            try
            {
                if (!string.IsNullOrEmpty(SearchFilter))
                {
                    SearchFilter = SearchFilter.Replace(",", "");
                    SearchFilter = SearchFilter.ToLower();
                    // SearchFilter = SearchFilter.Trim();
                    if (!string.IsNullOrEmpty(SearchFilter))
                    {
                        SearchFilter = SearchFilter.Replace(",", "");
                        SearchFilter = SearchFilter.ToLower();
                        // SearchFilter = SearchFilter.Trim();
                        SearchLog_AllFilter = SearchLog_All.ToList().Where(x => x.Object.ToString().ToLower().Contains(SearchFilter)
                       || (x.Action != null && x.Action.ToLower().Contains(SearchFilter))
                       || (x.OldValue!=null && x.OldValue.ToLower().Contains(SearchFilter))
                        || (x.NewValue != null && x.NewValue.ToLower().Contains(SearchFilter))
                         || (x.Remarks != null && x.Remarks.ToLower().Contains(SearchFilter))
                          || (x.PostedBy != null && x.PostedBy.ToLower().Contains(SearchFilter))
                           || (x.IP != null && x.IP.ToLower().Contains(SearchFilter))
                           || (x.DatePosted != null && x.DatePosted.Value.ToString("dd-MMM-yyyy").ToLower().Contains(SearchFilter))
                          ).OrderByDescending(x => x.ID).AsQueryable();
                    }
                    paginationObj = new PaginationDTO() { Page = 1, QuantityPerPage = paginationObj.QuantityPerPage };
                    PerPageSystemLog = SearchLog_AllFilter.Paginate(paginationObj).ToList();
                    // totalPageQuantity = ListOfDepositorsSearchFilter.Count() / 10;
                    totalPageQuantity = await SearchLog_AllFilter.InsertPaginationParameterInResponseForComponents(paginationObj.QuantityPerPage);
                    totalCount = SearchLog_AllFilter.Count();
                    currentPage = 1;
                }
                else
                {
                    paginationObj = new PaginationDTO() { Page = 1, QuantityPerPage = paginationObj.QuantityPerPage };
                    SearchLog_AllFilter = SearchLog_AllFilter.AsQueryable();
                    PerPageSystemLog = SearchLog_AllFilter.Paginate(paginationObj).ToList();
                    //totalPageQuantity = ListOfDepositorsSearchFilter.Count() / 10;
                    totalPageQuantity = await SearchLog_AllFilter.InsertPaginationParameterInResponseForComponents(paginationObj.QuantityPerPage);
                    totalCount = SearchLog_AllFilter.Count();
                    currentPage = 1;
                }
                // await LoadPeople();
                currentPage = 1;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}
