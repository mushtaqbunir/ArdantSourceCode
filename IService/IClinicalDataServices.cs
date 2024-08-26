using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.ClinicalData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ArdantOffical.IService
{
    public interface IClinicalDataServices
    {
        Task<Exception> AddDVANote(NotesVM NoteModel, string JobId);
        Task<Exception> AddHCPNote(NotesVM NoteModel, string JobId);
        Task<Exception> AddNDISNote(NotesVM NoteModel, string JobId);
        Task<Exception> AddNote(NotesVM NoteModel, string JobId);
        Task<Exception> DeleteDVANote(string Id);
        Task<Exception> DeleteHCPNote(string Id);
        Task<Exception> DeleteNDISNote(string Id);
        Task<Exception> DeleteNote(string Id);
        Task<NotesVM> GetDVANoteByID(string Id);
        Task<NotesVMForTable> GetDVANotes(string JobId);
        Task<NotesVM> GetHCPNoteByID(string Id);
        Task<NotesVMForTable> GetHCPNotes(string JobId);
        Task<NotesVM> GetNDISNoteByID(string Id);
        Task<NotesVMForTable> GetNDISNotes(string JobId);
        Task<NotesVM> GetNoteByID(string Id);
        Task<NotesVMForTable> GetNotes(string JobId);
        Task<Exception> UpdateDVANote(NotesVM NoteModel, string Id);
        Task<Exception> UpdateHCPNote(NotesVM NoteModel, string Id);
        Task<Exception> UpdateNDISNote(NotesVM NoteModel, string Id);
        Task<Exception> UpdateNote(NotesVM NoteModel, string Id);
    }
}
