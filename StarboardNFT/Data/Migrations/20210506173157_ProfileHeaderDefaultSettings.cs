using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class ProfileHeaderDefaultSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DefaultProfile",
                table: "ProfileSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileHeaderId",
                table: "ProfileSettings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ProfileHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileHeader", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSettings_ProfileHeaderId",
                table: "ProfileSettings",
                column: "ProfileHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileSettings_ProfileHeader_ProfileHeaderId",
                table: "ProfileSettings",
                column: "ProfileHeaderId",
                principalTable: "ProfileHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileSettings_ProfileHeader_ProfileHeaderId",
                table: "ProfileSettings");

            migrationBuilder.DropTable(
                name: "ProfileHeader");

            migrationBuilder.DropIndex(
                name: "IX_ProfileSettings_ProfileHeaderId",
                table: "ProfileSettings");

            migrationBuilder.DropColumn(
                name: "DefaultProfile",
                table: "ProfileSettings");

            migrationBuilder.DropColumn(
                name: "ProfileHeaderId",
                table: "ProfileSettings");
        }
    }
}
