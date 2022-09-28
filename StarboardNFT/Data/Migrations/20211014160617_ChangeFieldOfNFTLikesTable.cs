using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class ChangeFieldOfNFTLikesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NFTLikes_UserProfile_UserProfileId",
                table: "NFTLikes");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "NFTLikes",
                newName: "UserProfileHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_NFTLikes_UserProfileId",
                table: "NFTLikes",
                newName: "IX_NFTLikes_UserProfileHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_NFTLikes_UserProfileHeader_UserProfileHeaderId",
                table: "NFTLikes",
                column: "UserProfileHeaderId",
                principalTable: "UserProfileHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NFTLikes_UserProfileHeader_UserProfileHeaderId",
                table: "NFTLikes");

            migrationBuilder.RenameColumn(
                name: "UserProfileHeaderId",
                table: "NFTLikes",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_NFTLikes_UserProfileHeaderId",
                table: "NFTLikes",
                newName: "IX_NFTLikes_UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_NFTLikes_UserProfile_UserProfileId",
                table: "NFTLikes",
                column: "UserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
