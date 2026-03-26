using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessLearning.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTenantIdFromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
