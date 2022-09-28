using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class NFTUserProfileLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NFT_UserProfileId",
                table: "NFT",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_NFT_UserProfile_UserProfileId",
                table: "NFT",
                column: "UserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NFT_UserProfile_UserProfileId",
                table: "NFT");

            migrationBuilder.DropIndex(
                name: "IX_NFT_UserProfileId",
                table: "NFT");
        }
    }
}
