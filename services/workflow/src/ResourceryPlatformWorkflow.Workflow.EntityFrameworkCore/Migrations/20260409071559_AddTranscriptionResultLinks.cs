using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class AddTranscriptionResultLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkDocx",
                table: "Transcriptions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkHtml",
                table: "Transcriptions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkJson",
                table: "Transcriptions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkSrt",
                table: "Transcriptions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkTxt",
                table: "Transcriptions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkVerbatimDocx",
                table: "Transcriptions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkDocx",
                table: "Transcriptions");

            migrationBuilder.DropColumn(
                name: "LinkHtml",
                table: "Transcriptions");

            migrationBuilder.DropColumn(
                name: "LinkJson",
                table: "Transcriptions");

            migrationBuilder.DropColumn(
                name: "LinkSrt",
                table: "Transcriptions");

            migrationBuilder.DropColumn(
                name: "LinkTxt",
                table: "Transcriptions");

            migrationBuilder.DropColumn(
                name: "LinkVerbatimDocx",
                table: "Transcriptions");
        }
    }
}
