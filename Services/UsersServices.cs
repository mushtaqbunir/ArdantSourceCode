using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Data.ModelVm.Invoices;
using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Data.ModelVm.Users;
using ArdantOffical.Helpers;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Google.Authenticator;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesforceSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ArdantOffical.Services
{
    public class UsersServices : ControllerBase, IUsersServices
    {
        private readonly FGCDbContext context;
        public static string keyy { get; set; }
        private ICreateIAuthorizationPolicy CreatePolicy;
        private readonly AuthenticationStateProvider UserauthenticationStateProvider;
        IServiceScopeFactory _serviceScope;
        public ISystemConfiguration SConfig;
        public string ImageUrl { get; set; }
        public string Text { get; set; }



        public UsersServices(FGCDbContext context, AuthenticationStateProvider _UserauthenticationStateProvider, IServiceScopeFactory serviceScope, ISystemConfiguration _SConfig)
        {
            this.context = context;
            //var gg = myConfiguration.Value.token;
            this.UserauthenticationStateProvider = _UserauthenticationStateProvider;
            _serviceScope = serviceScope;
            this.SConfig = _SConfig;
            //this.CreatePolicy = _CreatePolicy;
        }
        public bool ChangePassword(AddVM user)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public async Task<UserVmUseForTable> GetUsers([FromQuery] PaginationDTO pagination)
        {
            try
            {

                UserVmUseForTable listOfUsers = new UserVmUseForTable();
                var queryable = (from c in context.TblUsers.OrderBy(x => x.Firstname).ThenBy(x => x.Lastname)
                                 where c.IsDelete == false
                                 && c.UserStatus == 1
                                 //&& c.Email.Contains("fgc-capital.com")
                                 select new UserVM
                                 {
                                     UserID = c.UserId,
                                     UserKey = c.UserKey,
                                     Firstname = c.Firstname,
                                     Lastname = c.Lastname,
                                     ShortName = c.ShortName,
                                     Name = c.Firstname.Trim() + " " + c.Lastname.Trim(),
                                     Email = c.Email,
                                     Username = c.Username,
                                     Password = c.Password,
                                     EnableTwoFactor = c.EnableTwoFactor,
                                     RoleID = context.UserRoles.Where(x => x.UserId == c.UserId && x.IsDelete == false).Select(x => x.RoleId).FirstOrDefault(),
                                     UserStatus = c.UserStatus,
                                     OnlineStatus = c.OnlineStatus,
                                     SkipAuthenticator = c.SkipAuthenticator
                                 });
                listOfUsers.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                listOfUsers.UserVmList = queryable.Paginate(pagination).ToList();
                listOfUsers.TotalCount = queryable.Count();
                //double queryableCount = context.TblUsers.Count();
                //listOfUsers.TotalPages = await queryableCount.InsertPaginationParameterInResponse22(pagination.QuantityPerPage);
                //listOfUsers.UserVmIQueryable = queryable;
                //listOfUsers.TotalCount = Convert.ToInt32(queryableCount);
                return listOfUsers;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        [HttpGet]
        public async Task<UserVmUseForTable> GetAllOTs([FromQuery] PaginationDTO pagination)
        {
            try
            {

                UserVmUseForTable listOfUsers = new UserVmUseForTable();
                var queryable = (from c in context.TblUsers.OrderBy(x => x.Firstname).ThenBy(x => x.Lastname)
                                 join ur in context.UserRoles on c.UserId equals ur.UserId
                                 join r in context.Roles on ur.RoleId equals r.Id
                                 where c.IsDelete == false
                                 && c.UserStatus == 1
                                 && r.Name == "OT"
                                 //&& c.Email.Contains("fgc-capital.com")
                                 select new UserVM
                                 {
                                     UserID = c.UserId,
                                     UserKey = c.UserKey,
                                     Firstname = c.Firstname,
                                     Lastname = c.Lastname,
                                     ShortName = c.ShortName,
                                     Name = c.Firstname.Trim() + " " + c.Lastname.Trim(),
                                     Email = c.Email,
                                     Username = c.Username,
                                     Password = c.Password,
                                     SalesforceId = c.SalesforceID,
                                     EnableTwoFactor = c.EnableTwoFactor,
                                     RoleID = context.UserRoles.Where(x => x.UserId == c.UserId && x.IsDelete == false).Select(x => x.RoleId).FirstOrDefault(),
                                     UserStatus = c.UserStatus,
                                     OnlineStatus = c.OnlineStatus,
                                     SkipAuthenticator = c.SkipAuthenticator
                                 });
                listOfUsers.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                listOfUsers.UserVmList = queryable.Paginate(pagination).ToList();
                listOfUsers.TotalCount = queryable.Count();
                //double queryableCount = context.TblUsers.Count();
                //listOfUsers.TotalPages = await queryableCount.InsertPaginationParameterInResponse22(pagination.QuantityPerPage);
                //listOfUsers.UserVmIQueryable = queryable;
                //listOfUsers.TotalCount = Convert.ToInt32(queryableCount);
                return listOfUsers;
            }
            catch (Exception e)
            {
                throw;
            }
        }



        [HttpPost]
        public async Task<Exception> SaveUser(AddVM user)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // string uref = Reference.GetUniqueReference($"FGC");
                    Models.TblUser record = new();
                    //record.UserKey = user.UserKey;
                    record.Firstname = user.Firstname;
                    //record.ShortName = user.ShortName;
                    record.Lastname = user.Lastname;
                    record.Email = user.Email;
                    record.Username = user.Username;
                    record.Password = user.Password;
                    record.UserRole = user.UserRole;
                    record.UserStatus = user.UserStatus;
                    record.SalesforceID = user.SalesforceID;
                    record.OnlineStatus = user.OnlineStatus;
                    context.TblUsers.Add(record);
                    context.Entry(record).State = EntityState.Added;
                    await context.TblUsers.AddAsync(record);
                    if (context.SaveChanges() > 0)
                    {
                        Models.UserRole userRole = new Models.UserRole();
                        userRole.UserId = record.UserId;
                        userRole.RoleId = Convert.ToInt32(record.UserRole);
                        await context.UserRoles.AddAsync(userRole);
                    }

                    //create UserClaim
                    int RoleId = Convert.ToInt32(record.UserRole);
                    var checkRoleClaimsList = await context.RoleClaims.Where(x => x.RoleId == RoleId).ToListAsync();

                    List<UserClaim> ListOfUserClaim = new();

                    foreach (var item in checkRoleClaimsList)
                    {
                        var MenuRecord = await context.MenuItems.Where(x => x.MenuName == item.Value).FirstOrDefaultAsync();

                        //use for all checkbox checked 
                        var AddNewMenuItemll = new UserClaim
                        {
                            Type = "All-" + MenuRecord.MenuName,
                            Value = "All-" + MenuRecord.MenuName,
                            UserId = record.UserId,
                            MenuItemParentID = 0
                        };
                        ListOfUserClaim.Add(AddNewMenuItemll);
                        var AddNewMenuItemfirst = new UserClaim
                        {
                            Type = MenuRecord.MenuName,
                            Value = MenuRecord.MenuName,
                            UserId = record.UserId,
                            MenuItemParentID = MenuRecord.MenuItemParentID
                        };
                        ListOfUserClaim.Add(AddNewMenuItemfirst);


                        var ListOfMenu = await context.MenuItems.Where(x => x.MenuItemParentID == MenuRecord.MenuItemID).ToListAsync();
                        foreach (var menuitem in ListOfMenu)
                        {
                            var AddNewMenuItem = new UserClaim
                            {
                                Type = menuitem.MenuName,
                                Value = menuitem.MenuName,
                                UserId = record.UserId,
                                MenuItemParentID = menuitem.MenuItemParentID
                            };
                            ListOfUserClaim.Add(AddNewMenuItem);
                        }
                    }
                    await context.UserClaims.AddRangeAsync(ListOfUserClaim);


                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        [HttpPost]
        public async Task<Exception> UpdateUser(EditVM user, int UserID)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    //string UserKey = Reference.GetUniqueReference($"FGC");
                    Models.TblUser record = context.TblUsers.Find(UserID);
                    if (!string.IsNullOrEmpty(user.UserKey))
                    {
                        if (record.UserKey.ToLower() != user.UserKey.ToLower())
                        {
                            record.EnableTwoFactor = false;
                        }
                    }
                    //record.UserKey = user.UserKey;
                    record.Firstname = user.Firstname;
                    record.Lastname = user.Lastname;
                    record.ShortName = user.ShortName;
                    record.Email = user.Email;
                    record.Username = user.Username;
                    record.UserRole = user.UserRole;
                    record.Password = user.Password;
                    record.UserStatus = user.UserStatus;
                    record.OnlineStatus = user.OnlineStatus;
                    int RoleID = Convert.ToInt32(user.UserRole);
                    var id = context.UserRoles.Where(x => x.UserId == record.UserId && x.IsDelete == false).Select(x => x.Id).FirstOrDefault();
                    if (id == 0) // Add New entry in the User Role Table
                    {
                        Models.UserRole role = new Models.UserRole();
                        role.RoleId = RoleID;
                        role.UserId = UserID;
                        context.Entry(role).State = EntityState.Added;
                        context.UserRoles.Add(role);
                    }
                    else // Update User Role 
                    {
                        Models.UserRole userRole = context.UserRoles.Find(id);
                        userRole.RoleId = RoleID;
                        context.Entry(record).State = EntityState.Modified;
                    }
                    /// This code is commented 
                    //var CheckRecord = await context.UserClaims.Where(x => x.UserId == UserID).ToListAsync();//Remove all user claims
                    //if (CheckRecord != null)
                    //{
                    //    context.UserClaims.RemoveRange(CheckRecord);
                    //    await context.SaveChangesAsync();
                    //}

                    //int RoleId = Convert.ToInt32(record.UserRole);
                    //var checkRoleClaimsList = await context.RoleClaims.Where(x => x.RoleId == RoleId).ToListAsync();//check role claims

                    //List<UserClaim> ListOfUserClaim = new();

                    //foreach (var item in checkRoleClaimsList)//add user claims
                    //{
                    //    var MenuRecord = await context.MenuItems.Where(x => x.MenuName == item.Value).FirstOrDefaultAsync();//get parent Id From  menu table

                    //    //use for all checkbox checked 
                    //    var AddNewMenuItemll = new UserClaim
                    //    {
                    //        Type = "All-" + MenuRecord.MenuName,
                    //        Value = "All-" + MenuRecord.MenuName,
                    //        UserId = record.UserId,
                    //        MenuItemParentID = 0
                    //    };
                    //    ListOfUserClaim.Add(AddNewMenuItemll);
                    //    var AddNewMenuItem = new UserClaim
                    //    {
                    //        Type = MenuRecord.MenuName,
                    //        Value = MenuRecord.MenuName,
                    //        UserId = record.UserId,
                    //        MenuItemParentID = MenuRecord.MenuItemParentID
                    //    };
                    //    ListOfUserClaim.Add(AddNewMenuItem);

                    //    var ListOfMenu = await context.MenuItems.Where(x => x.MenuItemParentID == MenuRecord.MenuItemID).ToListAsync();
                    //    foreach (var menuitem in ListOfMenu)
                    //    {
                    //        var AddNewMenuItemFirst = new UserClaim
                    //        {
                    //            Type = menuitem.MenuName,
                    //            Value = menuitem.MenuName,
                    //            UserId = record.UserId,
                    //            MenuItemParentID = menuitem.MenuItemParentID
                    //        };
                    //        ListOfUserClaim.Add(AddNewMenuItemFirst);
                    //    }
                    //}
                    //await context.UserClaims.AddRangeAsync(ListOfUserClaim);

                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }

        }
        public async Task<Exception> DeleteUser(int UserID)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.TblUser user = context.TblUsers.Find(UserID);
                    user.IsDelete = true;
                    //context.TblUsers.Remove(user);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }


        public async Task<Exception> DeleteOT(string salesforceID)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                
                try
                {
                    SFConnect.client.Delete("OTs__c", salesforceID);
                    Models.TblUser user = context.TblUsers.Where(u => u.SalesforceID == salesforceID).FirstOrDefault();
                    user.IsDelete = true;
                    //context.TblUsers.Remove(user);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public bool CheckAvailability(string Username)
        {
            var queryable = context.TblUsers.Where(x => x.Username == Username && x.IsDelete == false)
                                            .ToList();
            bool IsUsernameAvailable = queryable.Count > 0;
            return IsUsernameAvailable;
        }
        public ChangePassVM GetUserPasswordByID(int id)
        {
            var queryable = (from c in context.TblUsers
                             where c.UserId == id && c.IsDelete == false
                             select new ChangePassVM
                             {
                                 UserID = c.UserId,
                                 Password = c.Password

                             }).AsQueryable().FirstOrDefault();

            return queryable;
        }
        public EditVM GetUserByID(int id)
        {
            var queryable = (from c in context.TblUsers
                             where c.UserId == id && c.IsDelete == false
                             select new EditVM
                             {
                                 UserID = c.UserId,
                                 UserKey = c.UserKey,
                                 Password = c.Password,
                                 Firstname = c.Firstname,
                                 ShortName = c.ShortName,
                                 Lastname = c.Lastname,
                                 Name = c.Firstname + " " + c.Lastname,
                                 Email = c.Email,
                                 Username = c.Username,
                                 UserRole = c.UserRole,
                                 RoleID = context.UserRoles.Where(x => x.UserId == c.UserId && x.IsDelete == false).Select(x => x.RoleId).FirstOrDefault(),
                                 UserStatus = c.UserStatus,
                                 OnlineStatus = c.OnlineStatus

                             }).AsQueryable().FirstOrDefault();

            return queryable;
        }
        public async Task<Exception> ChangePassword(ChangePassVM user, int UserID)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.TblUser record = context.TblUsers.Find(UserID);
                    record.Password = user.NewPassword;
                    context.Entry(record).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public bool CheckOldPassword(string oldPassword)
        {
            throw new NotImplementedException();
        }
        public async Task<CurrentUserInfoVM> GetCurrentUserInfo()
        {

            CurrentUserInfoVM CurrentuserInfo = new CurrentUserInfoVM();
            CurrentuserInfo = await UserauthenticationStateProvider.CurrentUser();//httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Sid).FirstOrDefault();  //ClaimsPrincipal.Current.Identity.Name;

            return CurrentuserInfo;
        }
        [HttpGet]
        public async Task<UserVmUseForTable> GetUserSearch([FromQuery] PaginationDTO pagination, string SearchKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = SearchKey.ToLower();
                    SearchKey = SearchKey.Trim();
                }
                UserVmUseForTable listOfUsers = new UserVmUseForTable();
                var queryable = (from c in context.TblUsers
                                 join u in context.UserRoles on c.UserId equals u.UserId
                                 where c.IsDelete == false
                                 select new UserVM
                                 {
                                     UserID = c.UserId,
                                     UserKey = c.UserKey,
                                     Firstname = c.Firstname,
                                     Lastname = c.Lastname,
                                     Name = c.Firstname.Trim() + " " + c.Lastname.Trim(),
                                     Email = c.Email,
                                     Username = c.Username,
                                     Password = c.Password,
                                     EnableTwoFactor = c.EnableTwoFactor,
                                     ShortName = c.ShortName,
                                     // RoleID = context.UserRoles.Where(x => x.UserId == c.UserId).Select(x => x.RoleId).FirstOrDefault(),
                                     RoleID = u.RoleId,
                                     UserStatus = c.UserStatus,
                                     OnlineStatus = c.OnlineStatus,
                                     SkipAuthenticator = c.SkipAuthenticator,
                                     RoleName = (from x in context.Roles where x.Id == u.RoleId && x.IsDelete == false select x.Name).FirstOrDefault()
                                 }).ToList();
                queryable = (from c in queryable
                             where (string.IsNullOrEmpty(SearchKey)
                                || c.UserID.ToString().ToLower().Trim().Contains(SearchKey)
                                || (c.Name != null && c.Name.ToLower().Trim().Contains(SearchKey))
                                || (c.Email != null && c.Email.ToLower().Trim().Contains(SearchKey))
                                || (c.Username != null && c.Username.ToLower().Contains(SearchKey))
                                || (c.RoleName != null && c.RoleName.ToString().Trim().ToLower().Contains(SearchKey))
                                || (c.UserStatusForShow != null && c.UserStatusForShow.ToString().Trim().ToLower().Contains(SearchKey))
                                || (c.UserStatus != null && c.UserStatus.ToString().Trim().ToLower().Contains(SearchKey))
                                || (c.OnlineStatus != null && c.OnlineStatus.ToLower().Trim().Contains(SearchKey))
                                || (c.ShortName != null && c.ShortName.ToLower().Trim().Contains(SearchKey))
                                )
                             select c).OrderBy(x => x.Name).ToList();
                var queryablee = queryable.AsQueryable();

                listOfUsers.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryablee, pagination.QuantityPerPage);
                listOfUsers.UserVmList = queryablee.Paginate(pagination).ToList();
                listOfUsers.TotalCount = queryablee.Count();

                return listOfUsers;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        [HttpGet]
        public async Task<List<MenuItem>> GetMenuItemList()
        {
            try
            {
                List<MenuItem> listOfMenuItem = new();
                listOfMenuItem = await context.MenuItems.OrderBy(x => x.SortOrder)
                .Include(x => x.MenuItems)
                .Include(v => v.MenuItemChild).ToListAsync();
                return listOfMenuItem;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<MenuItem>> GetOnBoardingMenuItemList()
        {
            try
            {
                List<MenuItem> listOfMenuItem = new();
                listOfMenuItem = await context.MenuItems.OrderBy(x => x.MenuItemParentID)
                .Include(x => x.MenuItems)
                .Include(v => v.MenuItemChild).ToListAsync();
                //listOfMenuItem = listOfMenuItem.Where(s => s.MenuName == "On Boarding Panel Menu").ToList();
                return listOfMenuItem;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IQueryable<MenuItem> GetMenuItemIQueryable()
        {
            try
            {

                IQueryable<MenuItem> listOfMenuItem = context.MenuItems.OrderBy(x => x.MenuItemParentID).Where(x => x.MenuItemParentID != null);
                return listOfMenuItem;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        [HttpGet]
        public async Task<List<ListOfMenuItemFormModel>> GetUserMenuItemList(int userbyid)
        {
            try
            {
                List<ListOfMenuItemFormModel> listOfMenuItem = await (from s in context.UserClaims
                                                                      where s.UserId == userbyid
                                                                      select new ListOfMenuItemFormModel
                                                                      {
                                                                          MenuName = s.Value,
                                                                          MenuItemParentID = s.MenuItemParentID
                                                                      }).ToListAsync();
                return listOfMenuItem;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        [HttpGet]
        public async Task<string[]> GetUserMenuItemList()
        {
            try
            {
                var authstate = UserauthenticationStateProvider.GetAuthenticationStateAsync();
                var User = authstate.Result.User;
                if (!User.Identity.IsAuthenticated) return new string[0];

                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                return await context.UserClaims.Where(x => x.UserId == Userinfo.UserId).Select(x => x.Value).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        public async Task<Exception> AddMenuAccess(MenuItemFormModel MI)
        {

            try
            {
                var CheckRecord = await context.UserClaims.Where(x => x.UserId == MI.UserId).ToListAsync();
                if (CheckRecord != null)
                {
                    context.UserClaims.RemoveRange(CheckRecord);
                    await context.SaveChangesAsync();
                }
                foreach (var item in MI.ListOfMenuItems)
                {
                    var AddNewMenuItem = new UserClaim
                    {
                        Type = item.MenuName,
                        Value = item.MenuName,
                        UserId = MI.UserId,
                        // MenuItemParentID= item.MenuName.Contains("All-") ? 0: MI.MenuItemParentID
                        MenuItemParentID = item.MenuItemParentID
                    };
                    await context.UserClaims.AddAsync(AddNewMenuItem);
                    await context.SaveChangesAsync();
                }
                return new Ok("2");
            }
            catch (Exception ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }
        [HttpGet]
        public async Task<MenuItemCheckboxVM> GetMenuItemAccessByUser(int userid)
        {
            try
            {
                MenuItemCheckboxVM MenuItemCheckbox = new MenuItemCheckboxVM();
                var GetMenuItemAccess = await (from s in context.UserClaims
                                               where s.UserId == userid
                                               select s).ToListAsync();
                return MenuItemCheckbox;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<MenuItemUseForTable> GetMenuItemNavMenuLink([FromQuery] PaginationDTO pagination, string SearchFilter = "")
        {
            try
            {
                MenuItemUseForTable listOfMenuItem = new();
                var queryable = (from m in context.MenuItems
                                 where m.MenuItemParentID != null && m.IsParent == true && m.IsDelete == false
                                 select new MenuItemVM
                                 {
                                     MenuItemID = m.MenuItemID,
                                     MenuName = m.MenuName,
                                     ActionLink = m.ActionLink,
                                     Icons = m.Icons,
                                     IsParent = m.IsParent,
                                     IsDelete = m.IsDelete,
                                     MenuItemParentID = m.MenuItemParentID.Value
                                 });
                if (!string.IsNullOrEmpty(SearchFilter))
                {
                    queryable = queryable.ToList().Where(x => (x.MenuItemID.ToString().ToLower().Contains(SearchFilter))
                              || (x.MenuName != null && x.MenuName.ToLower().Contains(SearchFilter))
                              || (x.ActionLink != null && x.ActionLink.ToLower().Contains(SearchFilter))
                              || (x.Icons != null && x.Icons.ToLower().Contains(SearchFilter))
                              ).AsQueryable();
                }

                listOfMenuItem.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                listOfMenuItem.MenuItemList = queryable.Paginate(pagination).ToList();
                listOfMenuItem.TotalCount = queryable.Count();
                return listOfMenuItem;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Exception> DeleteMenuItem(int MenuItemID)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.MenuItem menu = context.MenuItems.Find(MenuItemID);
                    menu.IsDelete = true;
                    //context.TblUsers.Remove(user);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public async Task<Exception> AddNavMenuLink(MenuItemVM mil)
        {

            try
            {
                if (string.IsNullOrEmpty(mil.ActionLink))
                {
                    mil.ActionLink = "#";
                }
                var updareRecord = await context.MenuItems.Where(x => x.MenuItemID == mil.MenuItemID).FirstOrDefaultAsync();

                if (updareRecord == null)
                {
                    var AddNewMenuItemParent = new MenuItem //GrantParent for Menu
                    {
                        MenuName = mil.MenuName + " Menu",
                        Type = mil.MenuItemType,
                    };
                    await context.MenuItems.AddAsync(AddNewMenuItemParent);
                    var AddNewMenuItem = new MenuItem
                    {
                        MenuName = mil.MenuName,
                        ActionLink = mil.ActionLink,
                        Icons = mil.Icons,
                        IsParent = true,
                        Level = 1,
                        MenuItems = AddNewMenuItemParent,
                        Type = mil.MenuItemType
                    };
                    await context.MenuItems.AddAsync(AddNewMenuItem);
                    await context.SaveChangesAsync();
                    return new Ok("1");
                }
                else
                {
                    updareRecord.MenuName = mil.MenuName;
                    updareRecord.ActionLink = mil.ActionLink;
                    updareRecord.Icons = mil.Icons;
                    //updareRecord.IsDelete = true;
                    updareRecord.Level = 1;
                    updareRecord.Type = mil.MenuItemType;
                    await context.SaveChangesAsync();
                    return new Ok("2");
                }

            }
            catch (Exception ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }
        public async Task<Exception> AddSubNavMenuLink(MenuItemVM mil)
        {

            try
            {
                if (string.IsNullOrEmpty(mil.ActionLink))
                {
                    mil.ActionLink = "#";
                }
                var updareRecord = await context.MenuItems.Where(x => x.MenuItemID == mil.MenuItemID).FirstOrDefaultAsync();

                if (updareRecord == null)
                {

                    var AddNewMenuItem = new MenuItem
                    {
                        MenuName = mil.MenuName,
                        ActionLink = mil.ActionLink,
                        Icons = mil.Icons,
                        //IsParent = true,
                        //  Level = 1,
                        MenuItemParentID = mil.MenuItemParentID,
                        Type = mil.MenuItemType

                    };
                    await context.MenuItems.AddAsync(AddNewMenuItem);
                    await context.SaveChangesAsync();
                    return new Ok("1");
                }
                else
                {
                    updareRecord.MenuName = mil.MenuName;
                    updareRecord.ActionLink = mil.ActionLink;
                    updareRecord.Icons = mil.Icons;
                    updareRecord.Type = mil.MenuItemType;
                    //updareRecord.IsDelete = true;
                    //updareRecord.Level = 1;
                    await context.SaveChangesAsync();
                    return new Ok("2");
                }

            }
            catch (Exception ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }

        }
        //public async Task<Exception> AddNavMenuLink(MenuItemVM mil)
        //{

        //    try
        //    {
        //        if (string.IsNullOrEmpty(mil.ActionLink))
        //        {
        //            mil.ActionLink = "#";
        //        }
        //        var updareRecord = await context.MenuItems.Where(x => x.MenuItemID == mil.MenuItemID).FirstOrDefaultAsync();

        //        if (updareRecord == null)
        //        {
        //            if (mil.MenuItemGrantParentId == null && mil.MenuItemParentID == 0 && mil.MenuItemChildId == null)
        //            {
        //                var AddNewMenuItemParent = new MenuItem //GrantParent for Menu
        //                {
        //                    MenuName = mil.MenuName + " Menu",
        //                };
        //                await context.MenuItems.AddAsync(AddNewMenuItemParent);
        //                var AddNewMenuItem = new MenuItem
        //                {
        //                    MenuName = mil.MenuName,
        //                    ActionLink = mil.ActionLink,
        //                    Icons = mil.Icons,
        //                    Level = 1,
        //                    MenuItems = AddNewMenuItemParent
        //                };
        //                await context.MenuItems.AddAsync(AddNewMenuItem);
        //            }
        //            else if (mil.MenuItemGrantParentId != null && mil.MenuItemParentID == 0 && mil.MenuItemChildId == null)
        //            {
        //                var AddNewMenuItem = new MenuItem
        //                {
        //                    MenuName = mil.MenuName,
        //                    ActionLink = mil.ActionLink,
        //                    Level = 2,
        //                    Icons = mil.Icons,
        //                    MenuItemParentID = mil.MenuItemGrantParentId,
        //                };
        //                await context.MenuItems.AddAsync(AddNewMenuItem);
        //            }
        //            else if (mil.MenuItemGrantParentId != null && mil.MenuItemParentID != 0 && mil.MenuItemChildId == null)
        //            {
        //                var AddNewMenuItem = new MenuItem
        //                {
        //                    MenuName = mil.MenuName,
        //                    ActionLink = mil.ActionLink,
        //                    Icons = mil.Icons,
        //                    Level = 3,
        //                    MenuItemParentID = mil.MenuItemParentID,
        //                };
        //                await context.MenuItems.AddAsync(AddNewMenuItem);
        //            }
        //            else if (mil.MenuItemGrantParentId != null && mil.MenuItemParentID != 0 && mil.MenuItemChildId != null)
        //            {
        //                var AddNewMenuItem = new MenuItem
        //                {
        //                    MenuName = mil.MenuName,
        //                    ActionLink = mil.ActionLink,
        //                    Icons = mil.Icons,
        //                    Level = 4,
        //                    MenuItemParentID = mil.MenuItemChildId,
        //                };
        //                await context.MenuItems.AddAsync(AddNewMenuItem);
        //            }
        //            await context.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            if (mil.MenuItemGrantParentId == null && mil.MenuItemParentID == 0 && mil.MenuItemChildId == null)
        //            {
        //                updareRecord.MenuName = mil.MenuName;
        //                updareRecord.ActionLink = mil.ActionLink;
        //                updareRecord.Icons = mil.Icons;
        //                //updareRecord.IsDelete = true;
        //                updareRecord.Level = 1;

        //            }
        //            else if (mil.MenuItemGrantParentId != null && mil.MenuItemParentID == 0 && mil.MenuItemChildId == null)
        //            {
        //                updareRecord.MenuName = mil.MenuName;
        //                updareRecord.ActionLink = mil.ActionLink;
        //                updareRecord.Level = 2;
        //                updareRecord.Icons = mil.Icons;
        //                updareRecord.MenuItemParentID = mil.MenuItemGrantParentId;
        //            }
        //            else if (mil.MenuItemGrantParentId != null && mil.MenuItemParentID != 0 && mil.MenuItemChildId == null)
        //            {
        //                updareRecord.MenuName = mil.MenuName;
        //                updareRecord.ActionLink = mil.ActionLink;
        //                updareRecord.Icons = mil.Icons;
        //                updareRecord.Level = 3;
        //                updareRecord.MenuItemParentID = mil.MenuItemParentID;
        //            }
        //            else if (mil.MenuItemGrantParentId != null && mil.MenuItemParentID != 0 && mil.MenuItemChildId != null)
        //            {
        //                updareRecord.MenuName = mil.MenuName;
        //                updareRecord.ActionLink = mil.ActionLink;
        //                updareRecord.Icons = mil.Icons;
        //                updareRecord.Level = 4;
        //                updareRecord.MenuItemParentID = mil.MenuItemChildId;
        //            }

        //        }
        //        await context.SaveChangesAsync();
        //        return new Ok("2");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BadRequestException(ex.Message.ToString());
        //    }

        //}
        public async Task<MenuItem> NavMenuLinkByyId(int id)
        {



            MenuItem listOfMenuItem = await context.MenuItems.OrderBy(x => x.MenuItemParentID)
            .Include(x => x.MenuItems)
            .Include(v => v.MenuItemChild).Where(x => x.MenuItemID == id).FirstOrDefaultAsync();


            return listOfMenuItem;
        }
        public void addnew(int id)
        {
            var check = context.MenuItems.Where(x => x.MenuItemID == id).FirstOrDefault();
            check.IsDelete = false;
            context.SaveChanges();
        }
        public async Task<MenuItemUseForTable> GetFirstMenuItemNavMenuLink([FromQuery] PaginationDTO pagination, int MenuItemParentID, string SearchFilter = "")
        {
            try
            {
                MenuItemUseForTable listOfMenuItem = new();
                var queryable = (from m in context.MenuItems
                                 where m.MenuItemParentID != null && m.MenuItemParentID == MenuItemParentID && m.IsDelete == false
                                 select new MenuItemVM
                                 {
                                     MenuItemID = m.MenuItemID,
                                     MenuName = m.MenuName,
                                     ActionLink = m.ActionLink,
                                     Icons = m.Icons,
                                     IsParent = m.IsParent,
                                     IsDelete = m.IsDelete,
                                     MenuItemParentID = m.MenuItemParentID.Value
                                 });
                if (!string.IsNullOrEmpty(SearchFilter))
                {
                    queryable = queryable.ToList().Where(x => (x.MenuItemID.ToString().ToLower().Contains(SearchFilter))
                              || (x.MenuName != null && x.MenuName.ToLower().Contains(SearchFilter))
                              || (x.ActionLink != null && x.ActionLink.ToLower().Contains(SearchFilter))
                              || (x.Icons != null && x.Icons.ToLower().Contains(SearchFilter))
                              ).AsQueryable();
                }
                listOfMenuItem.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                listOfMenuItem.MenuItemList = queryable.Paginate(pagination).ToList();
                listOfMenuItem.TotalCount = queryable.Count();
                return listOfMenuItem;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<GenericVMUseForTable<RolesVM>> GetRoles([FromQuery] PaginationDTO pagination, string SearchFilter = "")
        {
            try
            {
                GenericVMUseForTable<RolesVM> listOfUsers = new();
                var queryable = (from c in context.Roles.OrderBy(x => x.Id)
                                 where c.IsDelete == false
                                 select new RolesVM
                                 {
                                     Id = c.Id,
                                     RoleName = c.Name,
                                     MeaningfulName = c.MeaningfulName
                                 });
                if (!string.IsNullOrEmpty(SearchFilter))
                {
                    queryable = queryable.ToList().Where(x => (x.Id.ToString().ToLower().Contains(SearchFilter))
                              || (x.RoleName != null && x.RoleName.ToLower().Contains(SearchFilter))
                              || (x.MeaningfulName != null && x.MeaningfulName.ToLower().Contains(SearchFilter))
                              ).AsQueryable();
                }
                listOfUsers.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                listOfUsers.ListOfData = queryable.Paginate(pagination).ToList();
                listOfUsers.TotalCount = queryable.Count();
                return listOfUsers;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<Exception> AddRole(RolesVM role)
        {
            try
            {
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                var updareRecord = await context.Roles.Where(x => x.Id == role.Id).FirstOrDefaultAsync();
                if (updareRecord == null)
                {
                    var AddNewRole = new Role
                    {
                        Name = role.RoleName,
                        MeaningfulName = role.MeaningfulName,
                    };
                    await context.Roles.AddAsync(AddNewRole);

                    List<RoleClaim> ListOfRoleClaim = new();
                    foreach (var item in role.ListOfRoleClaims)
                    {
                        var AddNewRoleClaims = new RoleClaim
                        {
                            Type = item.MenuName,
                            Value = item.MenuName,
                            Role = AddNewRole
                        };
                        ListOfRoleClaim.Add(AddNewRoleClaims);
                    }
                    await context.RoleClaims.AddRangeAsync(ListOfRoleClaim);



                    //List<MenuItem> ListOfMenuItem = new();
                    //List<string> ListofMenuName = new();

                    //ListOfMenuItem = await GetMenuItemList();

                    //foreach (var item in ListOfMenuItem.Where(s => s.MenuItemParentID == null).ToList())
                    //{
                    //    foreach (var firstItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == item.MenuItemID).ToList()))
                    //    {
                    //        ListofMenuName.Add(firstItem.MenuName);
                    //        foreach (var secondItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == firstItem.MenuItemID).ToList()))
                    //        {
                    //            ListofMenuName.Add(secondItem.MenuName);
                    //            foreach (var ThirdItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == secondItem.MenuItemID).ToList()))
                    //            {
                    //                ListofMenuName.Add(ThirdItem.MenuName);
                    //                foreach (var FouthItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == ThirdItem.MenuItemID).ToList()))
                    //                {
                    //                    ListofMenuName.Add(FouthItem.MenuName);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}



                    //TblFgclog FGCLog = new TblFgclog
                    //{
                    //    Object = "User Management",
                    //    Action = "Add Role",
                    //    //Remarks = $"Role : RoleName",
                    //    Remarks = $"New Role  addedd  ({role.RoleName})",
                    //    //Crid = checkExistRecord.Crid,
                    //    //CommentId = AddTblPrclog.Id,
                    //    IP = await SConfig.GetRemoteIpAddress(),
                    //    PostedBy = Userinfo.FullName + $"({Userinfo.Role})",
                    //    UserId = Userinfo.UserId,
                    //    DatePosted = DateTime.Now.DateTime_UK()
                    //};
                    //await context.TblFgclogs.AddAsync(FGCLog);
                    await context.SaveChangesAsync();
                    return new Ok("1");
                }
                else
                {
                    updareRecord.Name = role.RoleName;
                    updareRecord.MeaningfulName = role.MeaningfulName;
                    //use for role claims 
                    var useroleListID = await context.UserRoles.Where(x => x.RoleId == updareRecord.Id).Select(x => x.UserId).ToListAsync();
                    List<string> listofAddMenuList = new();
                    List<string> listofRemoveMenuList = new();
                    var DeleteRecord = await context.RoleClaims.Where(x => x.RoleId == updareRecord.Id).ToListAsync();
                    foreach (var itemm in role.ListOfRoleClaims)
                    {
                        if (!DeleteRecord.Any(x => x.Value == itemm.MenuName))
                            listofAddMenuList.Add(itemm.MenuName);
                    }
                    foreach (var itemm in DeleteRecord)
                    {
                        if (!role.ListOfRoleClaims.Any(x => x.MenuName == itemm.Value))
                            listofRemoveMenuList.Add(itemm.Value);
                    }
                    context.RoleClaims.RemoveRange(DeleteRecord);
                    //use for user claims
                    List<UserClaim> ListofAddUserClaim = new();
                    List<UserClaim> ListofRemoveUserClaim = new();
                    List<MenuItem> ListOfMenuItem = await GetMenuItemList();
                    foreach (var userid in useroleListID)
                    {
                        foreach (var item2 in listofAddMenuList)//use for add in userclaims table
                        {
                            foreach (var item in ListOfMenuItem.Where(s => s.MenuItemParentID == null).ToList())
                            {
                                foreach (var firstItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == item.MenuItemID && s.MenuName == item2).ToList()))
                                {
                                    var AddNewMenuItemll = new UserClaim { Type = "All-" + firstItem.MenuName, Value = "All-" + firstItem.MenuName, UserId = userid, MenuItemParentID = 0 };
                                    var AddNewMenuItem = new UserClaim { Type = firstItem.MenuName, Value = firstItem.MenuName, UserId = userid, MenuItemParentID = firstItem.MenuItemParentID };
                                    if (AddNewMenuItem != null)
                                        ListofAddUserClaim.Add(AddNewMenuItem);
                                    if (AddNewMenuItemll != null)
                                        ListofAddUserClaim.Add(AddNewMenuItemll);
                                    foreach (var secondItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == firstItem.MenuItemID).ToList()))
                                    {
                                        var AddNewMenusecondItem = new UserClaim { Type = secondItem.MenuName, Value = secondItem.MenuName, UserId = userid, MenuItemParentID = secondItem.MenuItemParentID };
                                        if (AddNewMenusecondItem != null)
                                            ListofAddUserClaim.Add(AddNewMenusecondItem);
                                        foreach (var ThirdItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == secondItem.MenuItemID).ToList()))
                                        {
                                            var AddNewMenuThirdItem = new UserClaim { Type = ThirdItem.MenuName, Value = ThirdItem.MenuName, UserId = userid, MenuItemParentID = ThirdItem.MenuItemParentID };
                                            if (AddNewMenuThirdItem != null)
                                                ListofAddUserClaim.Add(AddNewMenuThirdItem);
                                            foreach (var FouthItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == ThirdItem.MenuItemID).ToList()))
                                            {
                                                var AddNewMenuFouthItem = new UserClaim { Type = FouthItem.MenuName, Value = FouthItem.MenuName, UserId = userid, MenuItemParentID = FouthItem.MenuItemParentID };
                                                if (AddNewMenuFouthItem != null)
                                                    ListofAddUserClaim.Add(AddNewMenuFouthItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //use for Remove in userclaims table
                        foreach (var item2 in listofRemoveMenuList)
                        {
                            foreach (var item in ListOfMenuItem.Where(s => s.MenuItemParentID == null).ToList())
                            {
                                foreach (var firstItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == item.MenuItemID && s.MenuName == item2).ToList()))
                                {
                                    var menuName = "All-" + firstItem.MenuName;
                                    var removeUserClaimAll = await context.UserClaims.Where(x => x.Value == menuName && x.UserId == userid).FirstOrDefaultAsync();
                                    if (removeUserClaimAll != null)
                                        ListofRemoveUserClaim.Add(removeUserClaimAll);
                                    var removeUserClaim = await context.UserClaims.Where(x => x.Value == firstItem.MenuName && x.UserId == userid).FirstOrDefaultAsync();
                                    if (removeUserClaim != null)
                                        ListofRemoveUserClaim.Add(removeUserClaim);
                                    foreach (var secondItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == firstItem.MenuItemID).ToList()))
                                    {
                                        var DeleteuserClaim = await context.UserClaims.Where(x => x.Value == secondItem.MenuName && x.UserId == userid).FirstOrDefaultAsync();
                                        if (DeleteuserClaim != null)
                                            ListofRemoveUserClaim.Add(DeleteuserClaim);
                                        foreach (var ThirdItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == secondItem.MenuItemID).ToList()))
                                        {
                                            var DeleteThirdItemuserClaim = await context.UserClaims.Where(x => x.Value == ThirdItem.MenuName && x.UserId == userid).FirstOrDefaultAsync();
                                            if (DeleteThirdItemuserClaim != null)
                                                ListofRemoveUserClaim.Add(DeleteThirdItemuserClaim);
                                            foreach (var FouthItem in (ListOfMenuItem.Where(s => s.MenuItemParentID == ThirdItem.MenuItemID).ToList()))
                                            {
                                                var DeleteFouthItemItemuserClaim = await context.UserClaims.Where(x => x.Value == FouthItem.MenuName && x.UserId == userid).FirstOrDefaultAsync();
                                                if (DeleteFouthItemItemuserClaim != null)
                                                    ListofRemoveUserClaim.Add(DeleteFouthItemItemuserClaim);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }

                    context.UserClaims.RemoveRange(ListofRemoveUserClaim);
                    await context.UserClaims.AddRangeAsync(ListofAddUserClaim);
                    List<RoleClaim> ListOfRoleClaim = new();
                    foreach (var item in role.ListOfRoleClaims)
                    {
                        var AddNewRoleClaims = new RoleClaim
                        {
                            Type = item.MenuName,
                            Value = item.MenuName,
                            RoleId = updareRecord.Id
                        };
                        ListOfRoleClaim.Add(AddNewRoleClaims);
                    }
                    await context.RoleClaims.AddRangeAsync(ListOfRoleClaim);
                    //List<TblFgclog> ListOfTblFgclog = new();
                    //foreach (ChangeTrackerProperties item in context.ChangeTrackerDatabase<Role>())
                    //{
                    //    TblFgclog FGCLog = new TblFgclog
                    //    {
                    //        Object = "User Management",
                    //        Action = "Role updated",
                    //        Remarks = $"Role  {item.PropertyName} field has updated",
                    //        OldValue = item.OldValue,
                    //        NewValue = item.NewValue,
                    //        IP = await SConfig.GetRemoteIpAddress(),
                    //        PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                    //        UserId = Userinfo.UserId,
                    //        DatePosted = DateTime.Now.DateTime_UK()
                    //    };
                    //    ListOfTblFgclog.Add(FGCLog);
                    //}
                    //await context.TblFgclogs.AddRangeAsync(ListOfTblFgclog);
                    await context.SaveChangesAsync();
                    return new Ok("2");
                }
            }
            catch (Exception ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }
        public async Task<RolesVM> EditRole(int id)
        {
            try
            {
                var GetRoles = await (from s in context.Roles
                                      where s.Id == id
                                      select new RolesVM
                                      {
                                          Id = s.Id,
                                          RoleName = s.Name,
                                          MeaningfulName = s.MeaningfulName
                                      }).FirstOrDefaultAsync();
                return GetRoles;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetUserRole(int UserID)
        {
            try
            {
                string RoleID = context.TblUsers.Where(x => x.UserId == UserID).Select(x => x.UserRole).FirstOrDefault();
                return RoleID;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Exception> DeleteRole(int ID)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.Role role = context.Roles.Find(ID);
                    //user.IsDelete = true;
                    context.Roles.Remove(role);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("3");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public async Task<List<ListOfMenuItemFormModel>> GetRolesClaimsList(int RoleId)
        {
            try
            {
                //List<RoleClaim> crRole2s = (from rr in context.Roles
                //                            join cur in context.RoleClaims on rr.Id equals cur.RoleId
                //                            where cur.RoleId == RoleId
                //                            select cur).ToList();

                List<ListOfMenuItemFormModel> RoleClaims = await (from rr in context.Roles
                                                                  join rc in context.RoleClaims on rr.Id equals rc.RoleId
                                                                  where rc.RoleId == RoleId
                                                                  select new ListOfMenuItemFormModel
                                                                  {
                                                                      MenuName = rc.Value,
                                                                      MenuItemParentID = rc.Id
                                                                  }).ToListAsync();
                return RoleClaims;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        public async Task<string> GoogleAuthenticator(string UserEmail)
        {
            try
            {
                TblUser CheckEnableTwoFactor = context.TblUsers.Where(x => x.Username == UserEmail).FirstOrDefault();
                if (CheckEnableTwoFactor != null)
                    if (!CheckEnableTwoFactor.EnableTwoFactor)
                    {
                        // string key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
                        TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                        SetupCode setupInfo = tfa.GenerateSetupCode("FGC Two Factor", UserEmail, CheckEnableTwoFactor.UserKey, false, 3);
                        string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                        string manualEntrySetupCode = setupInfo.ManualEntryKey;
                        return qrCodeImageUrl;
                    }
                    else
                    {
                        return string.Empty;
                    }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public bool VerifyTFA(string code, string UserEmail)
        {
            bool result = false;
            TblUser CheckUser = context.TblUsers.Where(x => x.Username == UserEmail).FirstOrDefault();
            if (CheckUser != null)
            {
                // verify
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                //bool result = tfa.ValidateTwoFactorPIN(key, txtCode.Text);
                //result = tfa.ValidateTwoFactorPIN(keyy, code);
                result = tfa.ValidateTwoFactorPIN(CheckUser.UserKey, code, TimeSpan.FromSeconds(60));
            }
            else
            {
                result = false;
            }
            return result;
        }
        public async Task<Exception> EnableTwoFactorGoogleAuthenticator(int UserId)
        {
            try
            {
                TblUser CheckUser = await context.TblUsers.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
                if (CheckUser != null)
                {
                    CheckUser.EnableTwoFactor = false;
                    await context.SaveChangesAsync();
                    return new Ok("1");
                }
                else
                {
                    return new BadRequestException("1");
                }
            }
            catch (Exception ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }

        public async Task<Exception> SkipGoogleAuthenticator(int UserId, bool IsSkip)
        {
            try
            {
                TblUser CheckUser = await context.TblUsers.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
                if (CheckUser != null)
                {
                    CheckUser.SkipAuthenticator = IsSkip;
                    await context.SaveChangesAsync();
                    return new Ok("1");
                }
                else
                {
                    return new BadRequestException("1");
                }
            }
            catch (Exception ex)
            {
                return new BadRequestException(ex.Message.ToString());
            }
        }

        public async Task<bool> BankUserGroup()
        {
            try
            {
                CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                string[] Banks = new string[] { "LHV", "Modulr" };
                return !Banks.Contains(Userinfo.Role);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<List<AttachmentsVm>> GetOT_ProfilePic(string SFID)
        {
            try
            {
                return await (from u in context.TblUserAttachments
                              where u.SalesforceId == SFID
                              select new AttachmentsVm()
                              {
                                  ID = u.Id,
                                  Path = u.Path,
                                  Folder = u.FolderName,
                                  SalesforceID = u.SalesforceId,
                                  UserFileType = u.FileType,
                              }).ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<Exception> UpdateOT_Profile(List<AttachmentsVm> vm)
        {
            try
            {
                CurrentUserInfoVM CurrentuserInfo = new CurrentUserInfoVM();
                List<TblUsersAttachments> userFiles = new List<TblUsersAttachments>();
                CurrentuserInfo = await UserauthenticationStateProvider.CurrentUser();
                if (vm.Count > 0)
                {
                    foreach (var item in vm)
                    {
                        var existingFile = context.TblUserAttachments
                            .Where(ua => ua.SalesforceId == item.SalesforceID && ua.FileType == item.UserFileType)
                            .FirstOrDefault();

                        if (existingFile != null)
                        {
                            existingFile.Path = item.Path;
                            existingFile.Title = item.Title;
                            existingFile.FolderName = item.Folder;
                        }
                        else
                        {
                            var userFile = new TblUsersAttachments();
                            userFile.UserId = CurrentuserInfo.UserId;
                            userFile.SalesforceId = item.SalesforceID;
                            userFile.Path = item.Path;
                            userFile.FileType = item.UserFileType;
                            userFile.Title = item.Title;
                            userFile.FolderName = item.Folder;
                            userFiles.Add(userFile);
                        }
                    }
                    if (userFiles.Count > 0)
                    {
                        context.TblUserAttachments.AddRange(userFiles);
                    }
                }
                await context.SaveChangesAsync();

                return new Ok("1");
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
