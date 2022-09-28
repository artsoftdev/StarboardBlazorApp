using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class AddAuctionIDInRecentViewNFTTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuctionId",
                table: "RecentViewNFT",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RecentViewNFT_AuctionId",
                table: "RecentViewNFT",
                column: "AuctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecentViewNFT_Auction_AuctionId",
                table: "RecentViewNFT",
                column: "AuctionId",
                principalTable: "Auction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecentViewNFT_Auction_AuctionId",
                table: "RecentViewNFT");

            migrationBuilder.DropIndex(
                name: "IX_RecentViewNFT_AuctionId",
                table: "RecentViewNFT");

            migrationBuilder.DropColumn(
                name: "AuctionId",
                table: "RecentViewNFT");
        }
    }
}
