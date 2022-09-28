using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class ChangeRecentViewNFT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecentViewNFT_NFT_NFTId",
                table: "RecentViewNFT");

            migrationBuilder.RenameColumn(
                name: "NFTId",
                table: "RecentViewNFT",
                newName: "NFTDataId");

            migrationBuilder.RenameIndex(
                name: "IX_RecentViewNFT_NFTId",
                table: "RecentViewNFT",
                newName: "IX_RecentViewNFT_NFTDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecentViewNFT_NFTData_NFTDataId",
                table: "RecentViewNFT",
                column: "NFTDataId",
                principalTable: "NFTData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecentViewNFT_NFTData_NFTDataId",
                table: "RecentViewNFT");

            migrationBuilder.RenameColumn(
                name: "NFTDataId",
                table: "RecentViewNFT",
                newName: "NFTId");

            migrationBuilder.RenameIndex(
                name: "IX_RecentViewNFT_NFTDataId",
                table: "RecentViewNFT",
                newName: "IX_RecentViewNFT_NFTId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecentViewNFT_NFT_NFTId",
                table: "RecentViewNFT",
                column: "NFTId",
                principalTable: "NFT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
