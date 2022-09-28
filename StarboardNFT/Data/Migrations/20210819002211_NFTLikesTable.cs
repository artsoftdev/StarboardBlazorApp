using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class NFTLikesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FollowNFT");

            migrationBuilder.AddColumn<int>(
                name: "FavoriteCount",
                table: "NFT",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "NFT",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NFTFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NFTDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NFTFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NFTFavorites_NFTData_NFTDataId",
                        column: x => x.NFTDataId,
                        principalTable: "NFTData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_NFTFavorites_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "NFTLikes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NFTDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NFTLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NFTLikes_NFTData_NFTDataId",
                        column: x => x.NFTDataId,
                        principalTable: "NFTData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_NFTLikes_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NFTFavorites_NFTDataId",
                table: "NFTFavorites",
                column: "NFTDataId");

            migrationBuilder.CreateIndex(
                name: "IX_NFTFavorites_UserProfileId",
                table: "NFTFavorites",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_NFTLikes_NFTDataId",
                table: "NFTLikes",
                column: "NFTDataId");

            migrationBuilder.CreateIndex(
                name: "IX_NFTLikes_UserProfileId",
                table: "NFTLikes",
                column: "UserProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NFTFavorites");

            migrationBuilder.DropTable(
                name: "NFTLikes");

            migrationBuilder.DropColumn(
                name: "FavoriteCount",
                table: "NFT");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "NFT");

            migrationBuilder.CreateTable(
                name: "FollowNFT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NFTDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowNFT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowNFT_NFTData_NFTDataId",
                        column: x => x.NFTDataId,
                        principalTable: "NFTData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowNFT_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FollowNFT_NFTDataId",
                table: "FollowNFT",
                column: "NFTDataId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowNFT_UserProfileId",
                table: "FollowNFT",
                column: "UserProfileId");
        }
    }
}
