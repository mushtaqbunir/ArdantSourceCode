using FGCCore.Data.ModelVm;
using FGCCore.Data.ModelVm.ClientVM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FGCCore.Pages
{
    public partial class AccountStatement
    {

        [Parameter]
        public int? ClientID { get; set; }
        private int currentPage = 1;
        private int totalPageQuantity;
        private int totalCount = -1;
        public PaginationDTO paginationObj { get; set; }
        public bool IsloaderShow { get; set; } = false;
        public AccountStatementForTable lstMain { get; set; }
        public List<AccountStatementVM> lstPerPageRows { get; set; }
        public List<AccountStatementVM> lstAllRows { get; set; }
        public List<SelectListItem> ListOfTablePages = new List<SelectListItem>();
        public List<SelectListItem> lstOfClientAccounts = new List<SelectListItem>();
        AccountStatementSearchFilters SearchFilterModels = new AccountStatementSearchFilters();
        //Search
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }
        public void OnVisibilityChangedModel(bool visibilityStatus)
        {
            responseDialogVisibility = visibilityStatus;

        }

        protected override Task OnInitializedAsync()
        {
            try
            {
                // ListOfTablePages.Add(new SelectListItem() { Text = "25", Value = "25" });
                ListOfTablePages.Add(new SelectListItem() { Text = "50", Value = "50" });
                ListOfTablePages.Add(new SelectListItem() { Text = "75", Value = "75" });
                ListOfTablePages.Add(new SelectListItem() { Text = "100", Value = "100" });
                lstOfClientAccounts = clientrepo.GetClientAccounts(ClientID);
            }
            catch (Exception ex)
            {
                IsloaderShow = false;
                responseHeader = "ERROR";
                responseBody = ex.Message.ToString();
                responseDialogVisibility = true;
            }

            return Task.CompletedTask;
        }

        private async Task SelectedPage(int page)
        {
            currentPage = page;
            await LoadStatement(page, paginationObj.QuantityPerPage);
        }
        async Task LoadStatement(int page = 1, int quantityPerPage = 25)
        {
            try
            {
                IsloaderShow = true;
                currentPage = page;
                paginationObj = new PaginationDTO() { Page = page, QuantityPerPage = quantityPerPage };
                lstMain = await Ipayment2.GetClientAccountStatement(paginationObj, SearchFilterModels);
                if (lstMain != null)
                {
                    lstPerPageRows = lstMain.lstPerPageRows;
                    lstAllRows = lstMain.lstAllRows;
                    totalPageQuantity = lstMain.TotalPages;
                    totalCount = lstMain.TotalCount;
                }
                IsloaderShow = false;

            }
            catch (Exception ex)
            {
                IsloaderShow = false;
                responseHeader = "ERROR";
                responseBody = ex.Message.ToString();
                responseDialogVisibility = true;
            }

        }

        public async Task SubmitSearchFilter()
        {
                currentPage = 1;
               if (SearchFilterModels.EndDate >= SearchFilterModels.StartDate)
                {
                    await LoadStatement(1, paginationObj.QuantityPerPage);
                }
                else
                {
                    responseHeader = "ERROR";
                    responseBody = "Start date should not be greater than End Date";
                    responseDialogVisibility = true;
                }           

        }

        public async Task FormReset()
        {
            SearchFilterModels = new AccountStatementSearchFilters();
            await LoadStatement(1, paginationObj.QuantityPerPage);
        }

    }
}
