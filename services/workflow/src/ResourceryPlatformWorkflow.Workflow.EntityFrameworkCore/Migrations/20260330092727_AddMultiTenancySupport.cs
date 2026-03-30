using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ServiceWorkflowTasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "ServiceWorkflowSteps",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "ServiceWorkflowSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "ServiceWorkflowSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "ServiceWorkflowSteps",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceWorkflowSteps",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "ServiceWorkflowSteps",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "ServiceWorkflowSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ServiceWorkflowSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ServiceWorkflows",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ServiceWorkflowInstances",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "ServiceWorkflowHistory",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "ServiceWorkflowHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "ServiceWorkflowHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "ServiceWorkflowHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceWorkflowHistory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "ServiceWorkflowHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "ServiceWorkflowHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ServiceWorkflowHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Documents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowTasks_TenantId",
                table: "ServiceWorkflowTasks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowSteps_TenantId",
                table: "ServiceWorkflowSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflows_TenantId",
                table: "ServiceWorkflows",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowInstances_TenantId",
                table: "ServiceWorkflowInstances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowHistory_TenantId",
                table: "ServiceWorkflowHistory",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TenantId",
                table: "Requests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TenantId",
                table: "Documents",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceWorkflowTasks_TenantId",
                table: "ServiceWorkflowTasks");

            migrationBuilder.DropIndex(
                name: "IX_ServiceWorkflowSteps_TenantId",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropIndex(
                name: "IX_ServiceWorkflows_TenantId",
                table: "ServiceWorkflows");

            migrationBuilder.DropIndex(
                name: "IX_ServiceWorkflowInstances_TenantId",
                table: "ServiceWorkflowInstances");

            migrationBuilder.DropIndex(
                name: "IX_ServiceWorkflowHistory_TenantId",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropIndex(
                name: "IX_Requests_TenantId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Documents_TenantId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ServiceWorkflowTasks");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ServiceWorkflowSteps");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ServiceWorkflows");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ServiceWorkflowInstances");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ServiceWorkflowHistory");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Documents");
        }
    }
}
