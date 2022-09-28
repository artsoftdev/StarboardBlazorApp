using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class CreateAuctionAndChangeFiledsOfNFTData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EthBuyOutPrice",
                table: "NFTData");

            migrationBuilder.DropColumn(
                name: "EthStartPrice",
                table: "NFTData");

            migrationBuilder.DropColumn(
                name: "ReservePrice",
                table: "NFTData");

            migrationBuilder.AddColumn<decimal>(
                name: "FiatBuyOutPrice",
                table: "NFTData",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FiatReservePrice",
                table: "NFTData",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FiatStartPrice",
                table: "NFTData",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Auction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentBidPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxBidPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncrementAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsReserveMet = table.Column<bool>(type: "bit", nullable: false),
                    IsAuctionOver = table.Column<bool>(type: "bit", nullable: false),
                    NFTDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentWinningUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auction_NFTData_NFTDataId",
                        column: x => x.NFTDataId,
                        principalTable: "NFTData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Auction_UserProfileHeader_CurrentWinningUserId",
                        column: x => x.CurrentWinningUserId,
                        principalTable: "UserProfileHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "AuctionBid",
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
                    AuctionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BidUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionBid", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionBid_Auction_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionBid_UserProfileHeader_BidUserId",
                        column: x => x.BidUserId,
                        principalTable: "UserProfileHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auction_CurrentWinningUserId",
                table: "Auction",
                column: "CurrentWinningUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_NFTDataId",
                table: "Auction",
                column: "NFTDataId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionBid_AuctionId",
                table: "AuctionBid",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionBid_BidUserId",
                table: "AuctionBid",
                column: "BidUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionBid");

            migrationBuilder.DropTable(
                name: "Auction");

            migrationBuilder.DropColumn(
                name: "FiatBuyOutPrice",
                table: "NFTData");

            migrationBuilder.DropColumn(
                name: "FiatReservePrice",
                table: "NFTData");

            migrationBuilder.DropColumn(
                name: "FiatStartPrice",
                table: "NFTData");

            migrationBuilder.AddColumn<decimal>(
                name: "EthBuyOutPrice",
                table: "NFTData",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EthStartPrice",
                table: "NFTData",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReservePrice",
                table: "NFTData",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
