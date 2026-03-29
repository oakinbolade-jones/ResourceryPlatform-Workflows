using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class RemoveServiceWorkflowServiceRelationFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceWorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceWorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RelationServiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelationServiceWorkflowId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceWorkflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflowHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceWorkflowInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceWorkflowStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServiceWorkflowTaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Comment = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceWorkflowHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceWorkflowHistory_ServiceWorkflowInstances_ServiceWork~",
                        column: x => x.ServiceWorkflowInstanceId,
                        principalTable: "ServiceWorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflowTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceWorkflowInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceWorkflowStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    AssigneeUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceWorkflowTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceWorkflowTasks_ServiceWorkflowInstances_ServiceWorkfl~",
                        column: x => x.ServiceWorkflowInstanceId,
                        principalTable: "ServiceWorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceWorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceWorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceWorkflowSteps_ServiceWorkflows_ServiceWorkflowId",
                        column: x => x.ServiceWorkflowId,
                        principalTable: "ServiceWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_Name",
                table: "Services",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowHistory_PerformedAt",
                table: "ServiceWorkflowHistory",
                column: "PerformedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowHistory_ServiceWorkflowInstanceId",
                table: "ServiceWorkflowHistory",
                column: "ServiceWorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowHistory_ServiceWorkflowStepId",
                table: "ServiceWorkflowHistory",
                column: "ServiceWorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowHistory_ServiceWorkflowTaskId",
                table: "ServiceWorkflowHistory",
                column: "ServiceWorkflowTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowHistory_Type",
                table: "ServiceWorkflowHistory",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowInstances_CurrentStepId",
                table: "ServiceWorkflowInstances",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowInstances_RequestId",
                table: "ServiceWorkflowInstances",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowInstances_ServiceWorkflowId",
                table: "ServiceWorkflowInstances",
                column: "ServiceWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowInstances_Status",
                table: "ServiceWorkflowInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflows_Name",
                table: "ServiceWorkflows",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflows_RelationServiceId",
                table: "ServiceWorkflows",
                column: "RelationServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowSteps_ServiceWorkflowId",
                table: "ServiceWorkflowSteps",
                column: "ServiceWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowSteps_ServiceWorkflowId_Order",
                table: "ServiceWorkflowSteps",
                columns: new[] { "ServiceWorkflowId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowTasks_AssigneeUserId",
                table: "ServiceWorkflowTasks",
                column: "AssigneeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowTasks_ServiceWorkflowInstanceId",
                table: "ServiceWorkflowTasks",
                column: "ServiceWorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowTasks_ServiceWorkflowStepId",
                table: "ServiceWorkflowTasks",
                column: "ServiceWorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowTasks_Status",
                table: "ServiceWorkflowTasks",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "ServiceWorkflowHistory");

            migrationBuilder.DropTable(
                name: "ServiceWorkflowSteps");

            migrationBuilder.DropTable(
                name: "ServiceWorkflowTasks");

            migrationBuilder.DropTable(
                name: "ServiceWorkflows");

            migrationBuilder.DropTable(
                name: "ServiceWorkflowInstances");
        }
    }
}
