using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class AddTotalNumberAndUniqueNumberOfMintedNFT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniqueNumberOfMintedNFT",
                table: "NFTData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalNumberOfMintedNFT",
                table: "NFT",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueNumberOfMintedNFT",
                table: "NFTData");

            migrationBuilder.DropColumn(
                name: "TotalNumberOfMintedNFT",
                table: "NFT");
        }
    }
}
