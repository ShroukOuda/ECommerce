using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixCategorySubscriptionRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubscriptions_Categories_CategoryId1",
                table: "CategorySubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_CategorySubscriptions_CategoryId1",
                table: "CategorySubscriptions");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "CategorySubscriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId1",
                table: "CategorySubscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubscriptions_CategoryId1",
                table: "CategorySubscriptions",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubscriptions_Categories_CategoryId1",
                table: "CategorySubscriptions",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
