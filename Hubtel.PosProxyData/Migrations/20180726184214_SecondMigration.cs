using Microsoft.EntityFrameworkCore.Migrations;

namespace Hubtel.PosProxyData.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerPhoneNumber",
                table: "PaymentRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPhoneNumber",
                table: "PaymentRequests");
        }
    }
}
