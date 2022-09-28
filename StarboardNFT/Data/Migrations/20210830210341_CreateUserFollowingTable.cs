using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class CreateUserFollowingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFollowing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MainUserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowedUserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFollowing_UserProfile_FollowedUserProfileId",
                        column: x => x.FollowedUserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UserFollowing_UserProfile_MainUserProfileId",
                        column: x => x.MainUserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowing_FollowedUserProfileId",
                table: "UserFollowing",
                column: "FollowedUserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowing_MainUserProfileId",
                table: "UserFollowing",
                column: "MainUserProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollowing");
        }
    }
}
