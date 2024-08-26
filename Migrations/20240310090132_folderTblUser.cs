using Microsoft.EntityFrameworkCore.Migrations;

namespace ArdantOffical.Migrations
{
    public partial class folderTblUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Folder",
                schema: "dbo",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Folder",
                schema: "dbo",
                table: "Tbl_Users");
        }
    }
}
