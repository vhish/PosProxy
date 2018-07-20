using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hubtel.PosProxyData.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    PaymentType = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    SalesOrderId = table.Column<string>(nullable: true),
                    MomoPhoneNumber = table.Column<string>(nullable: true),
                    MomoChannel = table.Column<string>(nullable: true),
                    MomoToken = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TransactionSession = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true),
                    ExternalTransactionId = table.Column<string>(nullable: true),
                    AmountAfterCharges = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    ClientReference = table.Column<string>(maxLength: 255, nullable: true),
                    CustomerPaysFee = table.Column<bool>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    PosDevice = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerEmail = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    EmployeeName = table.Column<string>(nullable: true),
                    EmployeePhone = table.Column<string>(nullable: true),
                    EmployeeEmail = table.Column<string>(nullable: true),
                    BranchId = table.Column<string>(nullable: true),
                    BranchName = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_ClientReference",
                table: "PaymentRequests",
                column: "ClientReference",
                unique: true,
                filter: "[ClientReference] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentRequests");
        }
    }
}
