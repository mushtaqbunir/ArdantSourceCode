using Microsoft.EntityFrameworkCore.Migrations;

namespace ArdantOffical.Migrations
{
    public partial class ImageFieldsUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                schema: "dbo",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageTitle",
                schema: "dbo",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                schema: "dbo",
                table: "Tbl_Users");

            migrationBuilder.DropColumn(
                name: "ImageTitle",
                schema: "dbo",
                table: "Tbl_Users");
        }
    }
}
