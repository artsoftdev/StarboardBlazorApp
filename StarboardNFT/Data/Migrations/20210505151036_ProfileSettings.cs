using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class ProfileSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfileSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EthAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubscribedNewsletter = table.Column<bool>(type: "bit", nullable: false),
                    EmailNotification = table.Column<bool>(type: "bit", nullable: false),
                    TermsAgree = table.Column<bool>(type: "bit", nullable: false),
                    AccountFreeze = table.Column<bool>(type: "bit", nullable: false),
                    FreezeTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSettings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileSettings");
        }
    }
}
