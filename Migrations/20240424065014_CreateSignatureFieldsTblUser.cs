using Microsoft.EntityFrameworkCore.Migrations;

namespace ArdantOffical.Migrations
{
    public partial class CreateSignatureFieldsTblUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignaturePath",
                schema: "dbo",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureTitle",
                schema: "dbo",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignaturePath",
                schema: "dbo",
                table: "Tbl_Users");

            migrationBuilder.DropColumn(
                name: "SignatureTitle",
                schema: "dbo",
                table: "Tbl_Users");
        }
    }
}
