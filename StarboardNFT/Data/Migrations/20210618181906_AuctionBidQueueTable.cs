using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class AuctionBidQueueTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BidStatus",
                table: "AuctionBid",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuctionBidQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FiatBidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EthBidAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    MaxBidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FiatMaxBidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EthMaxBidAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    IsBuyItNow = table.Column<bool>(type: "bit", nullable: false),
                    IsAutoBid = table.Column<bool>(type: "bit", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    CreateTimeInTicks = table.Column<long>(type: "bigint", nullable: false),
                    AuctionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BidUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionBidQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionBidQueue_Auction_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionBidQueue_UserProfileHeader_BidUserId",
                        column: x => x.BidUserId,
                        principalTable: "UserProfileHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionBidQueue_AuctionId",
                table: "AuctionBidQueue",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionBidQueue_BidUserId",
                table: "AuctionBidQueue",
                column: "BidUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionBidQueue");

            migrationBuilder.DropColumn(
                name: "BidStatus",
                table: "AuctionBid");
        }
    }
}
