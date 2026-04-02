using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class RemoveServiceRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCenters_ServiceCenterId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "RelationServiceWorkflowId",
                table: "ServiceWorkflows");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Services",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services",
                column: "ServiceCenterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services");

            migrationBuilder.AddColumn<Guid>(
                name: "RelationServiceWorkflowId",
                table: "ServiceWorkflows",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services",
                column: "ServiceCenterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCenters_ServiceCenterId",
                table: "Services",
                column: "ServiceCenterId",
                principalTable: "ServiceCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
