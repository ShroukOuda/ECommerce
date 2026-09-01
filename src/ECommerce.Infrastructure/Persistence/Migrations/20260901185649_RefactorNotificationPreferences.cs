using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorNotificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserNotificationPreferences_UserId_Type_Channel",
                table: "UserNotificationPreferences");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "UserNotificationPreferences");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserNotificationPreferences");

            migrationBuilder.AddColumn<Guid>(
                name: "NotificationPreferenceId",
                table: "UserNotificationPreferences",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DefaultEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationPreferences_NotificationPreferenceId",
                table: "UserNotificationPreferences",
                column: "NotificationPreferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationPreferences_UserId_NotificationPreferenceId",
                table: "UserNotificationPreferences",
                columns: new[] { "UserId", "NotificationPreferenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_IsActive",
                table: "NotificationPreferences",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_Type_Channel",
                table: "NotificationPreferences",
                columns: new[] { "Type", "Channel" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotificationPreferences_NotificationPreferences_NotificationPreferenceId",
                table: "UserNotificationPreferences",
                column: "NotificationPreferenceId",
                principalTable: "NotificationPreferences",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNotificationPreferences_NotificationPreferences_NotificationPreferenceId",
                table: "UserNotificationPreferences");

            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserNotificationPreferences_NotificationPreferenceId",
                table: "UserNotificationPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserNotificationPreferences_UserId_NotificationPreferenceId",
                table: "UserNotificationPreferences");

            migrationBuilder.DropColumn(
                name: "NotificationPreferenceId",
                table: "UserNotificationPreferences");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "UserNotificationPreferences",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "UserNotificationPreferences",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationPreferences_UserId_Type_Channel",
                table: "UserNotificationPreferences",
                columns: new[] { "UserId", "Type", "Channel" },
                unique: true);
        }
    }
}
