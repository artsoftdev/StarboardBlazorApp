using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarboardNFT.Data.Migrations
{
    public partial class UpdateUserProfileHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountFreeze",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "DefaultProfile",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "EmailNotification",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "FreezeTime",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "SubscribedNewsletter",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "TermsAgree",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UserProfile");

            migrationBuilder.AddColumn<bool>(
                name: "AccountFreeze",
                table: "UserProfileHeader",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserProfileHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailNotification",
                table: "UserProfileHeader",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FreezeTime",
                table: "UserProfileHeader",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SubscribedNewsletter",
                table: "UserProfileHeader",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TermsAgree",
                table: "UserProfileHeader",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "UserBanner",
                table: "UserProfileHeader",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UserProfileHeader",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserOverview",
                table: "UserProfileHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "UserPhoto",
                table: "UserProfileHeader",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserSkills",
                table: "UserProfileHeader",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountFreeze",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "EmailNotification",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "FreezeTime",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "SubscribedNewsletter",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "TermsAgree",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "UserBanner",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "UserOverview",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "UserPhoto",
                table: "UserProfileHeader");

            migrationBuilder.DropColumn(
                name: "UserSkills",
                table: "UserProfileHeader");

            migrationBuilder.AddColumn<bool>(
                name: "AccountFreeze",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DefaultProfile",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailNotification",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FreezeTime",
                table: "UserProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SubscribedNewsletter",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TermsAgree",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UserProfile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
