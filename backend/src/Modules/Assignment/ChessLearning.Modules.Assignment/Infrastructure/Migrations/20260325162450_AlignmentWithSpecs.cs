using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessLearning.Modules.Assignment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignmentWithSpecs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearningPlanId",
                schema: "assignment",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "assignment",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                schema: "assignment",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "assignment",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "assignment",
                table: "Assignments",
                newName: "CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                schema: "assignment",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                schema: "assignment",
                table: "Assignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                schema: "assignment",
                table: "Assignments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                schema: "assignment",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                schema: "assignment",
                table: "Assignments",
                newName: "Description");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "assignment",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                schema: "assignment",
                table: "Assignments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "LearningPlanId",
                schema: "assignment",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                schema: "assignment",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                schema: "assignment",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "assignment",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
