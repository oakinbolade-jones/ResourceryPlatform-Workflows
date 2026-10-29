using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestTitle",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestTitle",
                table: "Requests");
        }
    }
}
