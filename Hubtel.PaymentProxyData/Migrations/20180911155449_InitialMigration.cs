using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hubtel.PaymentProxyData.Migrations
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
                    OrderId = table.Column<Guid>(nullable: true),
                    MomoPhoneNumber = table.Column<string>(nullable: true),
                    MomoChannel = table.Column<string>(nullable: true),
                    MomoToken = table.Column<string>(nullable: true),
                    TransactionSession = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true),
                    ExternalTransactionId = table.Column<string>(nullable: true),
                    AmountAfterCharges = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    ChargeCustomer = table.Column<bool>(nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PosDeviceId = table.Column<string>(nullable: true),
                    PosDeviceType = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    EmployeeName = table.Column<string>(nullable: true),
                    CustomerMobileNumber = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    BranchId = table.Column<string>(nullable: true),
                    BranchName = table.Column<string>(nullable: true),
                    IsSuccessful = table.Column<bool>(nullable: false),
                    ReceiptNumber = table.Column<string>(nullable: true),
                    OfflineGuid = table.Column<string>(maxLength: 50, nullable: true),
                    ClientReference = table.Column<string>(maxLength: 255, nullable: true),
                    Status = table.Column<string>(nullable: true),
                    OrderRequestDoc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderZipFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    Bucketname = table.Column<string>(nullable: true),
                    Filename = table.Column<string>(nullable: true),
                    MimeType = table.Column<string>(nullable: true),
                    Processed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderZipFiles", x => x.Id);
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

            migrationBuilder.DropTable(
                name: "SalesOrderZipFiles");
        }
    }
}
