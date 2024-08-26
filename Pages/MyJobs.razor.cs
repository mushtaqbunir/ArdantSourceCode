using ArdantOffical.Data.ModelVm.Dashboard;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using ArdantOffical.Models;
using ArdantOffical.Services;
using SalesforceSharp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using ArdantOffical.Helpers.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Components.Routing;

namespace ArdantOffical.Pages
{
    public partial class MyJobs
    {

        [Inject]
        public FGCDbContext Context { get; set; }
        public JobCardVM Job { get; set; } = new JobCardVM();
        public List<JobCardVM> NDISJobs { get; set; } = new List<JobCardVM>();
        public List<JobCardVM> HCPJobs { get; set; } = new List<JobCardVM>();
        public List<JobCardVM> DVAJobs { get; set; } = new List<JobCardVM>();
        public string JobID { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsloaderShow { get; set; } = false;
        private HubConnection? hubConnection;
        public EventCallback<bool> OnVisibilityChanged { get; set; }
        public bool responseDialogVisibility { get; set; }
        public string responseHeader { get; set; }
        public string responseBody { get; set; }

        public string GridStyle { get; set; }
        public string ChartTitle { get; set; }
        public bool IsLoading { get; set; } = false;
        public TostModel TostModelclass { get; set; } = new();
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        AuthenticationStateProvider UserauthenticationStateProvider { get; set; }

        public bool IsClinicalNoteSpinner { get; set; }
        public bool IsReferralSpinner { get; set; }
        public bool IsUploadFileSpinner { get; set; }
        public bool IsCreateInvoiceSpinner { get; set; }

        public bool AddSideBarVisibility { get; set; } = false;
        public void OnAddUserVisibilityChanged(bool visibilityStatus)
        {
            AddSideBarVisibility = visibilityStatus;
        }
        public bool UploadDocumentVisibility { get; set; }
        public string JobType { get; set; }

        public async Task ShowAddClinicalDataSideBar(string jobId, string type)
        {
            JobID = jobId;
            if (type == "NDIS")
            {
                IsClinicalNoteSpinner = true;
            }
            else if (type == "DVA")
            {
                IsDVAClinicalNoteSpinner = true;
            }
            else
            {
                IsHCPClinicalNoteSpinner = true;
            }

            await Task.Delay(30);
            JobType = type;
            OnAddUserVisibilityChanged(true);
            IsClinicalNoteSpinner = false;
            IsHCPClinicalNoteSpinner = false;
            IsDVAClinicalNoteSpinner = false;

        }
        public async Task ShowUploadDocumentsSideBar(string jobId, string type)
        {
            JobID = jobId;
            if (type == "NDIS")
            {
                IsUploadFileSpinner = true;
            }
            else if (type == "HCP")
            {
                IsHCPUploadFileSpinner = true;
            }
            else
            {
                IsDVAUploadFileSpinner = true;
            }
            await Task.Delay(100);
            UploadDocumentVisibility = true;
            IsUploadFileSpinner = false;
            IsHCPUploadFileSpinner = false;
            IsDVAUploadFileSpinner = false;
        }
        public void HideUploadDocumentsSideBar()
        {
            UploadDocumentVisibility = false;
        }


        public bool AddInvoiceVisibility { get; set; } = false;
        public void OnAddInvoiceVisibilityChanged(bool visibilityStatus)
        {
            AddInvoiceVisibility = visibilityStatus;
        }

        public void ShowAddInvoiceSideBar(string jobId, string type)
        {
            if (type == "HCP")
            {
                IsHCPCreateInvoiceSpinner = true;
            }
            else if (type == "DVA")
            {
                IsDVACreateInvoiceSpinner = true;
            }
            else
            {
                IsCreateInvoiceSpinner = true;
            }

            JobID = jobId;
            Navigator.LocationChanged += HandleLocationChanged;
            Navigator.NavigateTo($"/AddInvoice/{jobId}");
        }
        public int DVACounts { get; set; }
        public async Task LoadDVAJobs()
        {
            try
            {
                //if (!SFConnect.client.IsAuthenticated)
                //{
                    SFConnect.OpenConnection();
                //}
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<DVA>("SELECT Id, Condition_to_be_treated__c,Status__c, Given_Name__c, Email__c, Phone__c,CreatedDate, Date_Allocated__c,Postcode__c ,State__c  FROM DVA_Jobs__c  WHERE Status__c ='Assigned' AND OT__c='" + Userinfo.SalesforceID + "' ORDER BY Date_Allocated__c DESC  LIMIT 4");
                DVACounts = records.Count();
                foreach (var r in records.Take(3))
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Name = r.Given_Name__c;
                    Job.Status = r.Status__c;
                    Job.Postcode = r.Postcode__c;
                    Job.State = r.State__c;
                    Job.ShortDescription = (r.Condition_to_be_treated__c != null && r.Condition_to_be_treated__c.Length > 50) ? r.Condition_to_be_treated__c.Substring(0, 50) : r.Condition_to_be_treated__c;
                    Job.Description = r.Condition_to_be_treated__c;
                    Job.DatePosted = r.Date_Allocated__c.Value;
                    DVAJobs.Add(Job);

                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }
        public int HCPCounts { get; set; }
        public async Task LoadHCPJobs()
        {
            try
            {
                //if (!SFConnect.client.IsAuthenticated)
                //{
                    SFConnect.OpenConnection();
                //}
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<HCP>("SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,CreatedDate, Suburb__c, Postcode__c,State__c, Gender_Abbrev__c, Reason_for_Referral__c, Date_Allocated__c, Medical_Background__c  FROM HCP_Job__c   WHERE Status__c= 'Assigned' AND OT__c='" + Userinfo.SalesforceID + "'  ORDER BY Date_Allocated__c DESC  LIMIT 4");
                HCPCounts = records.Count();
                foreach (var r in records.Take(3))
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Gender = r.Gender_Abbrev__c;
                    Job.Age = SFConnect.CalculateAge(r.Date_of_Birth__c.Value);
                    Job.Postcode = r.Postcode__c;
                    Job.Suburb = r.Suburb__c;
                    Job.State = r.State__c;
                    Job.Name = r.Title__c + " " + r.P_Firstname__c + " " + r.P_Surname__c;
                    Job.Status = r.Status__c;
                    Job.ShortDescription = (r.Reason_for_Referral__c != null && r.Reason_for_Referral__c.Length > 50) ? r.Reason_for_Referral__c.Substring(0, 50) : r.Reason_for_Referral__c;
                    Job.Description = r.Reason_for_Referral__c;
                    Job.DiagnosedConditions = r.Medical_Background__c;
                    Job.ShortDC = (r.Medical_Background__c != null && r.Medical_Background__c.Length > 50) ? r.Medical_Background__c.Substring(0, 50) : r.Medical_Background__c;
                    Job.DatePosted = r.Date_Allocated__c.Value;
                    HCPJobs.Add(Job);

                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }

        public int NDISCount { get; set; }
        public async Task LoadNDISJobs()
        {
            try
            {
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<NDIS>("SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,CreatedDate, Suburb__c, Postcode__c,State__c, Date_Allocated__c,  Gender_Abbrev__c, Reason_for_Referral__c, Medical_Background__c  FROM NDIS_Job__c  WHERE Status__c= 'Assigned' AND OT__c='" + Userinfo.SalesforceID + "'  ORDER BY  Date_Allocated__c DESC  LIMIT 4");
                NDISCount = records.Count();
                foreach (var r in records.Take(3))
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Gender = r.Gender_Abbrev__c;
                    Job.Age = SFConnect.CalculateAge(r.Date_of_Birth__c.Value);
                    Job.Postcode = r.Postcode__c;
                    Job.Suburb = r.Suburb__c;
                    Job.State = r.State__c;
                    Job.Name = r.Title__c + " " + r.P_Firstname__c + " " + r.P_Surname__c;
                    Job.Status = r.Status__c;
                    Job.ShortDescription = (r.Reason_for_Referral__c != null && r.Reason_for_Referral__c.Length > 50) ? r.Reason_for_Referral__c.Substring(0, 50) : r.Reason_for_Referral__c;
                    Job.Description = r.Reason_for_Referral__c;
                    Job.DiagnosedConditions = r.Medical_Background__c;
                    Job.ShortDC = (r.Medical_Background__c != null && r.Medical_Background__c.Length > 50) ? r.Medical_Background__c.Substring(0, 50) : r.Medical_Background__c;
                    Job.DatePosted = r.Date_Allocated__c;
                    NDISJobs.Add(Job);

                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }
        protected override async Task OnInitializedAsync()
        {
            IsloaderShow = true;
            await Task.Delay(500);
            await LoadDVAJobs();
            await LoadHCPJobs();
            await LoadNDISJobs();
            IsloaderShow = false;
        }
        public async void OnAddSuccess(bool isAdded)
        {
            // Hide the form
            IsHCPJobDetail = false;
            IsNDISShowJobDetail = false;
            IsDVAJobDetail = false;
            if (isAdded)
            {
                await LoadNDISJobs();
                await LoadDVAJobs();
                await LoadHCPJobs();
                StateHasChanged();
            }
        }

        #region NDIS Job Detail
        public bool IsNDISShowJobDetail { get; set; }
        public async Task ShowNDISJobDetail(string jobId)
        {
            IsReferralSpinner = true;
            JobID = jobId;
            await Task.Delay(10);
            IsNDISShowJobDetail = true;
            IsReferralSpinner = false;
        }
        public void HideNDISJobDetail()
        {
            IsNDISShowJobDetail = false;
        }


        #endregion
        #region DVA Job Detail
        public bool IsDVAJobDetail { get; set; }
        public bool IsDVAClinicalNoteSpinner { get; set; }
        public bool IsDVAReferralFormSpinner { get; set; }
        public bool IsDVAUploadFileSpinner { get; set; }
        public bool IsDVACreateInvoiceSpinner { get; set; }
        public async Task ShowDVAJobDetail(string jobid)
        {
            JobID = jobid;
            IsDVAReferralFormSpinner = true;
           await Task.Delay(20);
            IsDVAJobDetail = true;
            IsDVAReferralFormSpinner = false;
        }
        public void HideDVAJobDetail()
        {
            IsDVAJobDetail = false;
        }
        #endregion

        #region HCP Job Detail
        public bool IsHCPJobDetail { get; set; }
        public bool IsHCPClinicalNoteSpinner { get; set; }
        public bool IsHCPReferralFormSpinner { get; set; }
        public bool IsHCPUploadFileSpinner { get; set; }
        public bool IsHCPCreateInvoiceSpinner { get; set; }
        public async Task ShowHCPJobDetail(string jobid)
        {
            IsHCPReferralFormSpinner = true;
            JobID = jobid;
            await Task.Delay(10);
            IsHCPJobDetail = true;
            IsHCPReferralFormSpinner = false;
        }
        public void HideHCPJobDetail()
        {
            IsHCPJobDetail = false;
        }
        #endregion

        #region DVA Show More Less
        public bool IsShowMore = true;
        public async Task LoadAllDVAJobs()
        {

            try
            {
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {

                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<DVA>("SELECT Id, Condition_to_be_treated__c,Status__c, Given_Name__c, Email__c, Phone__c,CreatedDate FROM DVA_Jobs__c  WHERE Status__c ='Assigned' AND OT__c='" + Userinfo.SalesforceID + "' ORDER BY CreatedDate ASC");
                var takeRecords = records.SkipLast(3);
                foreach (var r in takeRecords)
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Name = r.Given_Name__c;
                    Job.Status = r.Status__c;
                    Job.ShortDescription = (r.Condition_to_be_treated__c != null && r.Condition_to_be_treated__c.Length > 50) ? r.Condition_to_be_treated__c.Substring(0, 50) : r.Condition_to_be_treated__c;
                    Job.Description = r.Condition_to_be_treated__c;
                    Job.DatePosted = r.CreatedDate;
                    DVAJobs.Add(Job);

                }

            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }

        public async Task ShowMore()
        {

            IsloaderShow = true;
            await Task.Delay(500);
            await LoadAllDVAJobs();
            IsloaderShow = false;
            IsShowMore = false;
            StateHasChanged();
        }
        public async Task ShowLess()
        {
            IsShowMore = true;
            if (DVAJobs.Count > 3)
            {
                DVAJobs.RemoveRange(3, DVAJobs.Count - 3);
            }
        }

        #endregion

        #region NDIS Show More Less
        public bool IsNDISShowMore = true;
        public async Task LoadAllNDISJobs()
        {
            try
            {
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {

                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<NDIS>("SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,CreatedDate, Suburb__c, Postcode__c,State__c,  Gender_Abbrev__c, Reason_for_Referral__c  FROM NDIS_Job__c  WHERE Status__c= 'Assigned' AND OT__c='" + Userinfo.SalesforceID + "'  ORDER BY CreatedDate ASC");
                var takeRecords = records.SkipLast(3);
                foreach (var r in takeRecords)
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Gender = r.Gender_Abbrev__c;
                    Job.Age = SFConnect.CalculateAge(r.Date_of_Birth__c.Value);
                    //Job.Suburb = r.Suburb__c;
                    //Job.State = r.State__c;
                    Job.Name = r.Title__c + " " + r.P_Firstname__c + " " + r.P_Surname__c;
                    Job.Status = r.Status__c;
                    Job.ShortDescription = (r.Reason_for_Referral__c != null && r.Reason_for_Referral__c.Length > 50) ? r.Reason_for_Referral__c.Substring(0, 50) : r.Reason_for_Referral__c;
                    Job.Description = r.Reason_for_Referral__c;
                    Job.DatePosted = r.CreatedDate;
                    NDISJobs.Add(Job);

                }

            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }

        public async Task ShowNDISMore()
        {

            IsloaderShow = true;
            await Task.Delay(500);
            await LoadAllNDISJobs();
            IsloaderShow = false;
            IsNDISShowMore = false;
            StateHasChanged();
        }
        public async Task ShowNDISLess()
        {
            IsNDISShowMore = true;
            if (NDISJobs.Count > 3)
            {
                NDISJobs.RemoveRange(3, NDISJobs.Count - 3);
            }
        }

        #endregion

        #region HCP Show More Less
        public bool IsHCPShowMore = true;
        public async Task LoadAllHCPJobs()
        {
            try
            {
                if (!SFConnect.client.IsAuthenticated)
                {
                    SFConnect.OpenConnection();
                }
            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Authentication failed: {0} : {1}", ex.Error, ex.Message);
            }
            try
            {

                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<HCP>("SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,CreatedDate, Suburb__c, Postcode__c,State__c, Gender_Abbrev__c, Reason_for_Referral__c  FROM HCP_Job__c   WHERE Status__c= 'Assigned' AND OT__c='" + Userinfo.SalesforceID + "'  ORDER BY CreatedDate ASC");
                var takeRecords = records.SkipLast(3);
                foreach (var r in takeRecords)
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Gender = r.Gender_Abbrev__c;
                    Job.Age = SFConnect.CalculateAge(r.Date_of_Birth__c.Value);
                    //Job.Suburb = r.Suburb__c;
                    //Job.State = r.State__c;
                    Job.Name = r.Title__c + " " + r.P_Firstname__c + " " + r.P_Surname__c;
                    Job.Status = r.Status__c;
                    Job.ShortDescription = (r.Reason_for_Referral__c != null && r.Reason_for_Referral__c.Length > 50) ? r.Reason_for_Referral__c.Substring(0, 50) : r.Reason_for_Referral__c;
                    Job.Description = r.Reason_for_Referral__c;
                    Job.DatePosted = r.CreatedDate;
                    NDISJobs.Add(Job);

                }

            }
            catch (SalesforceException ex)
            {
                ErrorMessage = string.Format("Error : {0} : {1}", ex.Error, ex.Message);
            }
            // query records

        }

        public async Task ShowHCPMore()
        {
            IsloaderShow = true;
            await Task.Delay(500);
            await LoadAllHCPJobs();
            IsloaderShow = false;
            IsHCPShowMore = false;
            StateHasChanged();
        }
        public async Task ShowHCPLess()
        {
            IsHCPShowMore = true;
            if (HCPJobs.Count > 3)
            {
                HCPJobs.RemoveRange(3, HCPJobs.Count - 3);
            }
        }

        #endregion

        #region Subarb Detail Component
        public bool ShowDetailsVisibility { get; set; } = false;
        public string MessageDetail { get; set; } = "";
        public string HeaderTitle { get; set; } = "";
        public async Task ShowMessage(string message)
        {
            MessageDetail = message;
            HeaderTitle = "More Detials";
            OnShowMessageVisibilityChanged(true);
            StateHasChanged();
        }
        public void OnShowMessageVisibilityChanged(bool visibilityStatus)
        {
            ShowDetailsVisibility = visibilityStatus;
        }

        #endregion

        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            // Unsubscribe from the navigation completed event
            Navigator.LocationChanged -= HandleLocationChanged;

            // Hide the spinner after navigation is complete
            IsCreateInvoiceSpinner = false;
            IsHCPCreateInvoiceSpinner = false;
            IsDVACreateInvoiceSpinner = false;
        }
    }
}
