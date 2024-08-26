using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Dashboard;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.Models;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using SalesforceSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Pages
{
    public partial class Index
    {
       
        [Inject]
        public FGCDbContext Context { get; set; }
        public JobCardVM Job { get; set; } = new JobCardVM();
        public List<JobCardVM> NDISJobs { get; set; } = new List<JobCardVM>();
        public List<JobCardVM> HCPJobs { get; set; }= new List<JobCardVM>();
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

      
        
        public bool AddSideBarVisibility { get; set; } = false;
        public void OnAddUserVisibilityChanged(bool visibilityStatus)
        {
            AddSideBarVisibility = visibilityStatus;
        }

        public void ShowAddClinicalDataSideBar(string _jobID)
        {
            JobID = _jobID;
            OnAddUserVisibilityChanged(true);
        }
       
       public int DVACounts { get; set; }
        public async Task LoadDVAJobs()
        {
            await Task.Delay(10);
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
                DVAJobs = new List<JobCardVM>();
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<DVA>("SELECT Id, Condition_to_be_treated__c,Status__c, Given_Name__c, Email__c, Phone__c,CreatedDate,State__c,Dob__c, Postcode__c  FROM DVA_Jobs__c  WHERE Status__c IN ('Open')  ORDER BY CreatedDate DESC  LIMIT 4");
                foreach (var r in records)
                DVACounts = records.Count();
                foreach (var r in records.Take(3))
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Name = r.Given_Name__c;
                    Job.Status = r.Status__c;
                    Job.ShortDescription = (r.Condition_to_be_treated__c != null && r.Condition_to_be_treated__c.Length>50) ? r.Condition_to_be_treated__c.Substring(0, 50) : r.Condition_to_be_treated__c;
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


        public int HCPCounts { get; set; }
        public async Task LoadHCPJobs()
        {
           await Task.Delay(10);
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
                HCPJobs = new List<JobCardVM>();
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<HCP>("SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,CreatedDate, Suburb__c, Postcode__c,State__c, Gender_Abbrev__c, Reason_for_Referral__c, Medical_Background__c  FROM HCP_Job__c  WHERE Status__c IN ('Open')  ORDER BY CreatedDate DESC  LIMIT 4");
                HCPCounts = records.Count();
                foreach (var r in records)
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
                    Job.DatePosted = r.CreatedDate;
                    Job.DiagnosedConditions = r.Medical_Background__c;
                    Job.ShortDC = (r.Medical_Background__c != null && r.Medical_Background__c.Length > 50) ? r.Medical_Background__c.Substring(0, 50) : r.Medical_Background__c;
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
           await Task.Delay(10);
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
                NDISJobs = new List<JobCardVM>();
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var records = SFConnect.client.Query<NDIS>("SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,CreatedDate, Suburb__c, Postcode__c,State__c,  Gender_Abbrev__c, Reason_for_Referral__c, Medical_Background__c  FROM NDIS_Job__c  WHERE Status__c IN ('Open') ORDER BY CreatedDate DESC  LIMIT 4");
                foreach (var r in records)
                
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
                    Job.DatePosted = r.CreatedDate;
                    Job.DiagnosedConditions =r.Medical_Background__c;
                    Job.ShortDC = (r.Medical_Background__c != null && r.Medical_Background__c.Length > 50) ? r.Medical_Background__c.Substring(0, 50) : r.Medical_Background__c;
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
            try
            {
              
                   // IsloaderShow = true;
                    await LoadDVAJobs();
                    await LoadHCPJobs();
                    await LoadNDISJobs();
                  //  IsloaderShow = false;
            }
            catch (Exception ex)
            {
              //  IsloaderShow = false;
                ErrorMessage = string.Format("Error : {0} ", ex.Message);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ////try
            ////{
            ////    if(firstRender)
            ////    {
            ////        IsloaderShow = true;                   
            ////        await LoadDVAJobs();
            ////        await LoadHCPJobs();
            ////        await LoadNDISJobs();
            ////        IsloaderShow = false;
            ////    }
               


            ////}
            ////catch (Exception ex)
            ////{
            ////    IsloaderShow = false;
            ////    ErrorMessage = string.Format("Error : {0} ", ex.Message);
            ////}

        }
        

        public async void OnAddSuccess(bool isAdded)
        {
            // Hide the form
            IsHCPShowJobDetail = false;
            IsNDISShowJobDetail = false;
            IsShowJobDetail = false;
            if (isAdded)
            {                
                await LoadNDISJobs();
                await LoadDVAJobs();
                await LoadHCPJobs();
                StateHasChanged();
            }
        }

        #region DVA Single Job Detail

        public bool IsShowJobDetail { get; set; }
       
        public void ShowJobDetail(string jobId)
        {
            JobID = jobId;
            IsShowJobDetail = true;
        }
        public void HideJobDetail()
        {
            IsShowJobDetail = false;
        }

        #endregion

        #region HCP Single Job Detail

        public bool IsHCPShowJobDetail { get; set; }

        public void ShowHCPJobDetail(string jobId)
        {
            JobID = jobId;
            IsHCPShowJobDetail = true;
        }
        public void HideHCPJobDetail()
        {
            IsHCPShowJobDetail = false;
        }

        public bool IsNDISShowJobDetail { get; set; }
        
        public void ShowNDISJobDetail(string jobId)
        {
            JobID = jobId;
            IsNDISShowJobDetail = true;
        }
        public void HideNDISJobDetail()
        {
            IsNDISShowJobDetail = false;
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
                var records = SFConnect.client.Query<DVA>("SELECT Id, Condition_to_be_treated__c,Status__c, Given_Name__c, Email__c, Phone__c,CreatedDate,State__c,Dob__c, Postcode__c  FROM DVA_Jobs__c  WHERE Status__c IN ('Open')  ORDER BY CreatedDate ASC");
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
           // await Task.Delay(500);
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
                var records = SFConnect.client.Query<NDIS>("SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,CreatedDate, Suburb__c, Postcode__c,State__c,  Gender_Abbrev__c, Reason_for_Referral__c  FROM NDIS_Job__c  WHERE Status__c IN ('Open')  ORDER BY CreatedDate ASC");
                var takeRecords = records.SkipLast(3);
                foreach (var r in takeRecords)
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Gender = r.Gender_Abbrev__c;
                    Job.Age = SFConnect.CalculateAge(r.Date_of_Birth__c.Value);
                    Job.Suburb = r.Suburb__c;
                    Job.State = r.State__c;
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
           // await Task.Delay(500);
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
                var records = SFConnect.client.Query<HCP>("SELECT Id,P_Firstname__c, P_Surname__c, Title__c,Date_of_Birth__c,CreatedDate, Suburb__c, Postcode__c,State__c, Gender_Abbrev__c, Reason_for_Referral__c  FROM HCP_Job__c  WHERE Status__c IN ('Open')  ORDER BY CreatedDate ASC");
                var takeRecords = records.SkipLast(3);
                foreach (var r in takeRecords)
                {
                    Job = new JobCardVM();
                    Job.ID = r.Id;
                    Job.Gender = r.Gender_Abbrev__c;
                    Job.Age = SFConnect.CalculateAge(r.Date_of_Birth__c.Value);
                    Job.Suburb = r.Suburb__c;
                    Job.State = r.State__c;
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
          //  await Task.Delay(500);
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
    }




}
