using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class ChangeToEnumInReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportReason = table.Column<int>(type: "int", nullable: false),
                    ReporterEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsReportedClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsBanNeeded = table.Column<bool>(type: "bit", nullable: false),
                    IsNFTRemoved = table.Column<bool>(type: "bit", nullable: false),
                    ReportedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Report_UserProfileHeader_ReportedUserId",
                        column: x => x.ReportedUserId,
                        principalTable: "UserProfileHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedUserId",
                table: "Report",
                column: "ReportedUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Report");
        }
    }
}
