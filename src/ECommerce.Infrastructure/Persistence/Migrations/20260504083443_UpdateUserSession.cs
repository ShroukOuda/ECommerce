using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionToken",
                table: "UserSessions");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "UserSessions",
                newName: "RevokedAt");

            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UserSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "UserSessions",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceInfo",
                table: "UserSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                table: "UserSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UserSessions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiresAt",
                table: "UserSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ReplacedByToken",
                table: "UserSessions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceInfo",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiresAt",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "ReplacedByToken",
                table: "UserSessions");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "UserSessions",
                newName: "ExpiresAt");

            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UserSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "UserSessions",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45);

            migrationBuilder.AddColumn<string>(
                name: "SessionToken",
                table: "UserSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
