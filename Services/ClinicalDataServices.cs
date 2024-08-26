using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm.ClinicalData;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Helpers;
using ArdantOffical.IService;
using Microsoft.AspNetCore.Mvc;
using SalesforceSharp;
using System.Linq;
using System.Threading.Tasks;
using System;
using ArdantOffical.Models;
using ArdantOffical.Data.ModelVm.Users;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Spreadsheet;


namespace ArdantOffical.Services
{
    public class ClinicalDataServices : ControllerBase, IClinicalDataServices
    {
        private readonly FGCDbContext context;
        public const string IsDeleted = "Yes";
        public ClinicalDataServices(FGCDbContext context)
        {
            this.context = context;        
          
        }

        public Microsoft.AspNetCore.Http.HttpContext GetHttpContext()
        {
            return HttpContext;
        }


        [HttpGet]
        public async Task<NotesVMForTable> GetNotes(string JobId)
        {          
                NotesVMForTable lstNotes = new NotesVMForTable();
                try
                {                    
                    var records = SFConnect.client.Query<Notes>($"SELECT Id, Title,Body,ParentId FROM Note  WHERE  ParentId = '{JobId}'");
                    var queryable = (from c in records
                                     select new NotesVM
                                     {
                                         Id = c.Id,
                                         Title = c.Title,
                                         Body = c.Body,
                                       
                                     });
                  
                    lstNotes.Notes = queryable.ToList();
                    lstNotes.TotalCount = queryable.Count();                   
                    return lstNotes;

                }
                catch (SalesforceException ex)
                {
                    throw;
                }           
        }

        [HttpGet]
        public async Task<NotesVMForTable> GetNDISNotes(string JobId)
        {
            NotesVMForTable lstNotes = new NotesVMForTable();
            try
            {
                var records = SFConnect.client.Query<NDISNotes>($"SELECT Id, Name,Description__c,NDIS_Job_ID__c FROM NDIS_Notes__c  WHERE  NDIS_Job_ID__c = '{JobId}' AND IsDeleted__c !='Yes'");
                var queryable = (from c in records
                                 select new NotesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Name,
                                     Body= c.Description__c

                                 });

                lstNotes.Notes = queryable.ToList();
                lstNotes.TotalCount = queryable.Count();
                return lstNotes;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<NotesVMForTable> GetHCPNotes(string JobId)
        {
            NotesVMForTable lstNotes = new NotesVMForTable();
            try
            {
                var records = SFConnect.client.Query<HCPNotes>($"SELECT Id, Name,Description__c,HCP_Job_ID__c FROM HCP_Note__c  WHERE  HCP_Job_ID__c = '{JobId}' AND IsDeleted__c !='Yes'");
                var queryable = (from c in records
                                 select new NotesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Name,
                                     Body = c.Description__c

                                 });

                lstNotes.Notes = queryable.ToList();
                lstNotes.TotalCount = queryable.Count();
                return lstNotes;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<NotesVMForTable> GetDVANotes(string JobId)
        {
            NotesVMForTable lstNotes = new NotesVMForTable();
            try
            {
                var records = SFConnect.client.Query<DVANotes>($"SELECT Id, Name,Description__c,DVA_Job_ID__c FROM DVA_Note__c  WHERE  DVA_Job_ID__c = '{JobId}' AND IsDeleted__c !='Yes'");
                var queryable = (from c in records
                                 select new NotesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Name,
                                     Body = c.Description__c

                                 });

                lstNotes.Notes = queryable.ToList();
                lstNotes.TotalCount = queryable.Count();
                return lstNotes;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }


        public async Task<NotesVM> GetNoteByID(string Id)
        {         
            try
            {
                var records = SFConnect.client.Query<Notes>($"SELECT Id, Title,Body,ParentId FROM Note  WHERE  Id = '{Id}'");
                var queryable = (from c in records
                                 select new NotesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Title,
                                     Body = c.Body,

                                 }).FirstOrDefault();
         
                return queryable;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        public async Task<NotesVM> GetNDISNoteByID(string Id)
        {
            try
            {
                var records = SFConnect.client.Query<NDISNotes>($"SELECT Id, Name,Description__c,NDIS_Job_ID__c FROM NDIS_Notes__c   WHERE  Id = '{Id}'");
                var queryable = (from c in records
                                 select new NotesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Name,
                                     Body = c.Description__c,

                                 }).FirstOrDefault();

                return queryable;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        public async Task<NotesVM> GetHCPNoteByID(string Id)
        {
            try
            {
                var records = SFConnect.client.Query<HCPNotes>($"SELECT Id, Title,Description__c,HCP_Job_ID__c FROM HCP_Note__c   WHERE  Id = '{Id}'");
                var queryable = (from c in records
                                 select new NotesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Name,
                                     Body = c.Description__c,

                                 }).FirstOrDefault();

                return queryable;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        public async Task<NotesVM> GetDVANoteByID(string Id)
        {
            try
            {
                var records = SFConnect.client.Query<DVANotes>($"SELECT Id, Title,Description__c,DVA_Job_ID__c FROM DVA_Note__c   WHERE  Id = '{Id}'");
                var queryable = (from c in records
                                 select new NotesVM
                                 {
                                     Id = c.Id,
                                     Title = c.Name,
                                     Body = c.Description__c,

                                 }).FirstOrDefault();

                return queryable;

            }
            catch (SalesforceException ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<Exception> AddNote(NotesVM NoteModel, string JobId)
        {
            try
            {
                string result = SFConnect.client.Create("Note",
                  new
                  {
                      Title = NoteModel.Title,
                      Body = NoteModel.Body,
                      ParentId = JobId
                  });                
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }
        [HttpPost]
        public async Task<Exception> AddNDISNote(NotesVM NoteModel, string JobId)
        {
            try
            {
                string result = SFConnect.client.Create("NDIS_Notes__c",
                  new
                  {
                      Name = NoteModel.Title,
                      Description__c = NoteModel.Body,
                      NDIS_Job_ID__c = JobId,
                      IsDeleted__c="No"
                  });
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }

        [HttpPost]
        public async Task<Exception> AddHCPNote(NotesVM NoteModel, string JobId)
        {
            try
            {
                string result = SFConnect.client.Create("HCP_Note__c",
                  new
                  {
                      Name = NoteModel.Title,
                      Description__c = NoteModel.Body,
                      HCP_Job_ID__c = JobId,
                      IsDeleted__c = "No"
                  });
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }
        [HttpPost]
        public async Task<Exception> AddDVANote(NotesVM NoteModel, string JobId)
        {
            try
            {
                string result = SFConnect.client.Create("DVA_Note__c",
                  new
                  {
                      Name = NoteModel.Title,
                      Description__c = NoteModel.Body,
                      DVA_Job_ID__c = JobId,
                      IsDeleted__c = "No"
                  });
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }

        [HttpPost]
        public async Task<Exception> UpdateNote(NotesVM NoteModel, string Id)
        {
            try
            {
                   SFConnect.client.Update("Note", Id, new
                    {
                        Title = NoteModel.Title,
                        Body = NoteModel.Body
                    });
                    return new Ok("1");
               
            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }

        [HttpPost]
        public async Task<Exception> UpdateNDISNote(NotesVM NoteModel, string Id)
        {
            try
            {
                SFConnect.client.Update("NDIS_Notes__c", Id, new
                {
                    Name = NoteModel.Title,
                    Description__c = NoteModel.Body
                });
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }

        [HttpPost]
        public async Task<Exception> UpdateHCPNote(NotesVM NoteModel, string Id)
        {
            try
            {
                SFConnect.client.Update("HCP_Note__c", Id, new
                {
                    Name = NoteModel.Title,
                    Description__c = NoteModel.Body
                });
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }

        [HttpPost]
        public async Task<Exception> UpdateDVANote(NotesVM NoteModel, string Id)
        {
            try
            {
                SFConnect.client.Update("DVA_Note__c", Id, new
                {
                    Name = NoteModel.Title,
                    Description__c = NoteModel.Body
                });
                return new Ok("1");

            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }


        public async Task<Exception> DeleteNote(string Id)
        {
            try
            {
                SFConnect.client.Delete("Note", Id);
                return new Ok("1");
            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }

        public async Task<Exception> DeleteNDISNote(string Id)
        {
            try
            {
                SFConnect.client.Update("NDIS_Notes__c", Id, new
                {
                    IsDeleted__c = "Yes"
                });
                return new Ok("1");
            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }

        public async Task<Exception> DeleteHCPNote(string Id)
        {
            try
            {
                SFConnect.client.Update("HCP_Note__c", Id, new
                {
                    IsDeleted__c = "Yes"
                });
                return new Ok("1");
            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }

        public async Task<Exception> DeleteDVANote(string Id)
        {
            try
            {
                SFConnect.client.Update("DVA_Note__c", Id, new
                {
                    IsDeleted__c = "Yes"
                });
                return new Ok("1");
            }
            catch (SalesforceException ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }

    }
}
