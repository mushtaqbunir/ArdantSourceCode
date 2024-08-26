using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesforceSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArdantOffical.Pages.ArdantForms.Components
{
    public partial class ProviderRegistration
    {
        public OTVm OTModal { get; set; } = new OTVm();
        public AddVM UserModal = new AddVM();
        public string ErrorMessage { get; set; }
        public List<SelectListItem> lstTitles = new();

        public List<SelectListItem> lstGender = new();
        public bool IsloaderShow { get; set; } = false;
        [Inject]
        public NavigationManager NavManager { get; set; }
        public bool IsSpinner { get; set; }
        private void GetTitles()
        {
            lstTitles.Add(new SelectListItem() { Value = "Miss", Text = "Miss" });
            lstTitles.Add(new SelectListItem() { Value = "Mr.", Text = "Mr." });
            lstTitles.Add(new SelectListItem() { Value = "Ms.", Text = "Ms." });
            lstTitles.Add(new SelectListItem() { Value = "Mrs.", Text = "Mrs." });
            lstTitles.Add(new SelectListItem() { Value = "Dr.", Text = "Dr." });

        }

        private void GetGender()
        {
            lstGender.Add(new SelectListItem() { Value = "Female", Text = "Female" });
            lstGender.Add(new SelectListItem() { Value = "Male", Text = "Male" });
            lstGender.Add(new SelectListItem() { Value = "Non-binary", Text = "Non-binary" });
            lstGender.Add(new SelectListItem() { Value = "Prefer not to say", Text = "Prefer not to say" });
            lstGender.Add(new SelectListItem() { Value = "Other", Text = "Other" });

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
               
            }
           
        }

        protected override async Task OnInitializedAsync()
        {
            IsloaderShow = true;
            try
            {
                GetTitles();
                GetGender();
                IsloaderShow = false;
            }
            catch (System.Exception ex)
            {
                IsloaderShow = false;
                throw;
            }
        }

        public async Task SaveOTData()
        {

            // System.Net.ServicePointManager.SecurityProtocol =SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;           
            // all actions should be in a try-catch - i'll just do the authentication one for an example
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
            // Call the create method to create the record
            try
            {
                IsSpinner = true;
                await Task.Delay(10);
                // create a record using an anonymous class and returns the ID
                string result = SFConnect.client.Create("OTs__c", new { Firstname__c = OTModal.Firstname, Lastname__c = OTModal.Lastname, Email__c = OTModal.Email, Phone__c = OTModal.Phone, AHPRA_No__c = OTModal.AHPRANo, Gender__c = OTModal.Gender, Title__c = OTModal.Title });
                UserModal.Firstname = OTModal.Firstname;
                UserModal.Lastname = OTModal.Lastname;
                UserModal.Email = OTModal.Email;
                UserModal.UserRole = "12";  // User Role ID for OT
                UserModal.UserStatus = 1;
                UserModal.SalesforceID = result;
                await IuserServices.SaveUser(UserModal);


                /// Send Email to the Admin
                string EmailMessage = PopulateBody(OTModal);
                SendEmail(EmailMessage);
                OTModal = new OTVm();
                UserModal = new AddVM();
                IsSpinner = false;
                // ErrorMessage = "Thank you for your inquiry! We will get back to you within 48 hours.";
                NavManager.NavigateTo("/RegistrationConfirmation");

            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format("Failed to create record: " + ex.Message);
                IsSpinner = false;

            }

        }
        [Inject]
        IWebHostEnvironment Environment { get; set; }
        public string PopulateBody(OTVm t)
        {
            string templateFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/tpl/");
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(templateFolder, "email.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{#Title}", t.Title);
            body = body.Replace("{#Firstname}", t.Firstname);
            body = body.Replace("{#Lastname}", t.Lastname);
            body = body.Replace("{#Gender}", t.Gender);
            body = body.Replace("{#AHPRA}", t.AHPRANo);
            body = body.Replace("{#ABN}", t.ABN);
            body = body.Replace("{#Phone}", t.Phone);
            body = body.Replace("{#Email}", t.Email);

            return body;
        }

        private static async void SendEmail(string message)
        {
            List<string> lstEmails = new List<string>() {
                "mushtaqbunir@gmail.com",
                "access@ardant.com.au" };
            EmailService _emailSender = new EmailService();
            string subject = "New OT Registered !";
            await _emailSender.SendEmailAsync(lstEmails, subject, message);

        }

    }
}
