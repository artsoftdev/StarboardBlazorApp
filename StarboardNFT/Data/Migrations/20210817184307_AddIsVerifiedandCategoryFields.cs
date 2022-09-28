using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class AddIsVerifiedandCategoryFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "UserProfileHeader",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "category",
                table: "NFT",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "category",
                table: "NFT");
        }
    }
}
