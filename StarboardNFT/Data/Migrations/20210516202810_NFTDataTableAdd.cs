using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class NFTDataTableAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NFTData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSaleStarted = table.Column<bool>(type: "bit", nullable: false),
                    SaleStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SaleEndtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EthStartPrice = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    HasBuyoutPrice = table.Column<bool>(type: "bit", nullable: false),
                    EthBuyOutPrice = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    HasReservePrice = table.Column<bool>(type: "bit", nullable: false),
                    ReservePrice = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Royalty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EthPurchaseAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EthPurchaseAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    SalePurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    USDPurchaseAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NFTId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NFTData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NFTData_NFT_NFTId",
                        column: x => x.NFTId,
                        principalTable: "NFT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NFTData_NFTId",
                table: "NFTData",
                column: "NFTId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NFTData");
        }
    }
}
