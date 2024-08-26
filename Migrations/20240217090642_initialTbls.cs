using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArdantOffical.Migrations
{
    public partial class initialTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "ApiCredentials",
                schema: "dbo",
                columns: table => new
                {
                    AuthID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthKey = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Purpose = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Company = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Certificate = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    CertPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertThumbprint = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiCredentials", x => x.AuthID);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                schema: "dbo",
                columns: table => new
                {
                    MenuItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icons = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsParent = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuItemParentID = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.MenuItemID);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuItems_MenuItemParentID",
                        column: x => x.MenuItemParentID,
                        principalSchema: "dbo",
                        principalTable: "MenuItems",
                        principalColumn: "MenuItemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_APIErrorLog",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_APIErrorLog", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Role",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeaningfulName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Users",
                schema: "dbo",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Admin,Compliance,Auditor"),
                    Avatar = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UserStatus = table.Column<int>(type: "int", nullable: true, comment: "0=Blocked, 1=Approved/Allowed"),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PasswordReset = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OnlineStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnableTwoFactor = table.Column<bool>(type: "bit", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    SkipAuthenticator = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_RoleClaim",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tbl_RoleClaim_Tbl_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_UserRole",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tbl_UserRole_Tbl_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tbl_UserRole_Tbl_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuItemParentID = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IntroducerUserId = table.Column<int>(type: "int", nullable: false),
                    UsersUserId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Tbl_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Tbl_Role",
                columns: new[] { "Id", "IsDelete", "MeaningfulName", "Name" },
                values: new object[,]
                {
                    { 1, false, "Admin", "Admin" },
                    { 2, false, "Compliance", "Compliance" },
                    { 3, false, "Auditor", "Auditor" },
                    { 4, false, "Deputy Money Laundering Reporting Officer", "DMLRO" },
                    { 5, false, "Money Laundering Reporting Officer", "MLRO" },
                    { 6, false, "Finance", "Finance" },
                    { 7, false, "Internee", "Internee" },
                    { 8, false, "Business Relationship", "Business Relationship" },
                    { 9, false, "Operations", "Operations" },
                    { 10, false, "On-Boarding", "On-Boarding" },
                    { 11, false, "Compliance Commitee", "Compliance Commitee" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Tbl_Users",
                columns: new[] { "UserID", "Avatar", "City", "DateModified", "Designation", "Email", "EnableTwoFactor", "Firstname", "IsDelete", "Lastname", "OnlineStatus", "Password", "PasswordReset", "ShortName", "SkipAuthenticator", "UserKey", "UserRole", "UserStatus", "Username", "ZipCode" },
                values: new object[] { 1, null, null, null, "Administrator", "jsmith@gmail.com", false, "JOHN", false, "SMITH", null, "admin", null, null, false, "0a2w38de90123", null, 1, "jsmith@gmail.com", null });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Tbl_UserRole",
                columns: new[] { "Id", "IsDelete", "RoleId", "UserId" },
                values: new object[] { 1, false, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_MenuItemParentID",
                schema: "dbo",
                table: "MenuItems",
                column: "MenuItemParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_RoleClaim_RoleId",
                schema: "dbo",
                table: "Tbl_RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_UserRole_RoleId",
                schema: "dbo",
                table: "Tbl_UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_UserRole_UserId_RoleId",
                schema: "dbo",
                table: "Tbl_UserRole",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UsersUserId",
                schema: "dbo",
                table: "UserClaims",
                column: "UsersUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiCredentials",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MenuItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_APIErrorLog",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_RoleClaim",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_UserRole",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_Role",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_Users",
                schema: "dbo");
        }
    }
}
