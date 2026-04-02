using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceCenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceCenterId",
                table: "Services",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ServiceCenters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCenters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services",
                column: "ServiceCenterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCenters_Code",
                table: "ServiceCenters",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCenters_TenantId",
                table: "ServiceCenters",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCenters_ServiceCenterId",
                table: "Services",
                column: "ServiceCenterId",
                principalTable: "ServiceCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCenters_ServiceCenterId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "ServiceCenters");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServiceCenterId",
                table: "Services");
        }
    }
}
