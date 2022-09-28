using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class ChangeUserFollowingField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowing_UserProfile_MainUserProfileId",
                table: "UserFollowing");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowing_UserProfileHeader_FollowedUserProfileHeaderId",
                table: "UserFollowing");

            migrationBuilder.RenameColumn(
                name: "MainUserProfileId",
                table: "UserFollowing",
                newName: "MainUserProfileHeaderId");

            migrationBuilder.RenameColumn(
                name: "FollowedUserProfileHeaderId",
                table: "UserFollowing",
                newName: "FollowingUserProfileHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowing_MainUserProfileId",
                table: "UserFollowing",
                newName: "IX_UserFollowing_MainUserProfileHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowing_FollowedUserProfileHeaderId",
                table: "UserFollowing",
                newName: "IX_UserFollowing_FollowingUserProfileHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowing_UserProfileHeader_FollowingUserProfileHeaderId",
                table: "UserFollowing",
                column: "FollowingUserProfileHeaderId",
                principalTable: "UserProfileHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowing_UserProfileHeader_MainUserProfileHeaderId",
                table: "UserFollowing",
                column: "MainUserProfileHeaderId",
                principalTable: "UserProfileHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowing_UserProfileHeader_FollowingUserProfileHeaderId",
                table: "UserFollowing");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowing_UserProfileHeader_MainUserProfileHeaderId",
                table: "UserFollowing");

            migrationBuilder.RenameColumn(
                name: "MainUserProfileHeaderId",
                table: "UserFollowing",
                newName: "MainUserProfileId");

            migrationBuilder.RenameColumn(
                name: "FollowingUserProfileHeaderId",
                table: "UserFollowing",
                newName: "FollowedUserProfileHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowing_MainUserProfileHeaderId",
                table: "UserFollowing",
                newName: "IX_UserFollowing_MainUserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowing_FollowingUserProfileHeaderId",
                table: "UserFollowing",
                newName: "IX_UserFollowing_FollowedUserProfileHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowing_UserProfile_MainUserProfileId",
                table: "UserFollowing",
                column: "MainUserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowing_UserProfileHeader_FollowedUserProfileHeaderId",
                table: "UserFollowing",
                column: "FollowedUserProfileHeaderId",
                principalTable: "UserProfileHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
