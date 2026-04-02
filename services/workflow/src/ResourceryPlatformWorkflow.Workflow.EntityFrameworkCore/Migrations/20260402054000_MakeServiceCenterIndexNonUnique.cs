using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(WorkflowDbContext))]
    [Migration("20260402054000_MakeServiceCenterIndexNonUnique")]
    public partial class MakeServiceCenterIndexNonUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services");

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

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services",
                column: "ServiceCenterId",
                unique: true);
        }
    }
}
