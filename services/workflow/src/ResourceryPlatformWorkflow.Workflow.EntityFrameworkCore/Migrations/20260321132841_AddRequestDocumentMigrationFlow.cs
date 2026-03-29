using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestDocumentMigrationFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentMigrationStatus",
                table: "Requests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DocumentsPublishedAt",
                table: "Requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastMigrationError",
                table: "Documents",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MigratedAt",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MigrationStatus",
                table: "Documents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SharePointDocumentUrl",
                table: "Documents",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SharePointItemId",
                table: "Documents",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentMigrationStatus",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "DocumentsPublishedAt",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "LastMigrationError",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "MigratedAt",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "MigrationStatus",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SharePointDocumentUrl",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SharePointItemId",
                table: "Documents");
        }
    }
}
