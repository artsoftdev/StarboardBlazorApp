using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class UpdateUserFollowingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowing_UserProfile_FollowedUserProfileId",
                table: "UserFollowing");

            migrationBuilder.RenameColumn(
                name: "FollowedUserProfileId",
                table: "UserFollowing",
                newName: "FollowedUserProfileHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowing_FollowedUserProfileId",
                table: "UserFollowing",
                newName: "IX_UserFollowing_FollowedUserProfileHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowing_UserProfileHeader_FollowedUserProfileHeaderId",
                table: "UserFollowing",
                column: "FollowedUserProfileHeaderId",
                principalTable: "UserProfileHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowing_UserProfileHeader_FollowedUserProfileHeaderId",
                table: "UserFollowing");

            migrationBuilder.RenameColumn(
                name: "FollowedUserProfileHeaderId",
                table: "UserFollowing",
                newName: "FollowedUserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowing_FollowedUserProfileHeaderId",
                table: "UserFollowing",
                newName: "IX_UserFollowing_FollowedUserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowing_UserProfile_FollowedUserProfileId",
                table: "UserFollowing",
                column: "FollowedUserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
