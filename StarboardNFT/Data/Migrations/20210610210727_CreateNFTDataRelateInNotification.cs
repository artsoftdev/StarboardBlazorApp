using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class CreateNFTDataRelateInNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NFTDataId",
                table: "Notification",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Notification_NFTDataId",
                table: "Notification",
                column: "NFTDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_NFTData_NFTDataId",
                table: "Notification",
                column: "NFTDataId",
                principalTable: "NFTData",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_NFTData_NFTDataId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_NFTDataId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "NFTDataId",
                table: "Notification");
        }
    }
}
