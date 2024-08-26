using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.IService
{
    public interface IUsersServices
    {
        Task<UserVmUseForTable> GetUsers([FromQuery] PaginationDTO pagination);
        public EditVM GetUserByID(int id);
        public ChangePassVM GetUserPasswordByID(int id);
        public Task<Exception> SaveUser(AddVM user);
        public Task<Exception> UpdateUser(EditVM user, int UserID);
        public Task<Exception> ChangePassword(ChangePassVM user, int UserID);
        public Task<Exception> DeleteUser(int UserID);

        public Task<CurrentUserInfoVM> GetCurrentUserInfo();
        public bool CheckAvailability(string Username);
        public bool CheckOldPassword(string oldPassword);
        Task<UserVmUseForTable> GetUserSearch([FromQuery] PaginationDTO pagination, string SearchKey);
        Task<Exception> AddMenuAccess(MenuItemFormModel MI);
        Task<MenuItemCheckboxVM> GetMenuItemAccessByUser(int userid);
        Task<List<MenuItem>> GetMenuItemList();
        Task<List<MenuItem>> GetOnBoardingMenuItemList();

        Task<List<ListOfMenuItemFormModel>> GetUserMenuItemList(int userbyid);
        Task<string[]> GetUserMenuItemList();
        Task<List<ListOfMenuItemFormModel>> GetRolesClaimsList(int RoleId);



        Task<MenuItemUseForTable> GetMenuItemNavMenuLink([FromQuery] PaginationDTO pagination, string SearchFilter = "");
        Task<Exception> AddNavMenuLink(MenuItemVM mil);
        Task<Exception> AddSubNavMenuLink(MenuItemVM mil);
        IQueryable<MenuItem> GetMenuItemIQueryable();
        Task<MenuItem> NavMenuLinkByyId(int id);
        void addnew(int id);
        Task<MenuItemUseForTable> GetFirstMenuItemNavMenuLink([FromQuery] PaginationDTO pagination, int MenuItemParentID, string SearchFilter = "");
        Task<GenericVMUseForTable<RolesVM>> GetRoles([FromQuery] PaginationDTO pagination, string SearchFilter = "");
        Task<Exception> AddRole(RolesVM role);
        Task<RolesVM> EditRole(int id);
        Task<Exception> DeleteRole(int ID);
        Task<string> GoogleAuthenticator(string UserEmail);
        bool VerifyTFA(string code, string UserEmail);
        Task<Exception> EnableTwoFactorGoogleAuthenticator(int UserId);
        Task<bool> BankUserGroup();
        Task<Exception> DeleteMenuItem(int MenuItemID);
        string GetUserRole(int UserID);
        Task<Exception> SkipGoogleAuthenticator(int UserId, bool IsSkip);

        public Task<Exception> UpdateOT_Profile(List<AttachmentsVm> vm);
        public Task<List<AttachmentsVm>> GetOT_ProfilePic(string SFID);
        public Task<UserVmUseForTable> GetAllOTs([FromQuery] PaginationDTO pagination);
        public Task<Exception> DeleteOT(string salesforceID);
    }
}
