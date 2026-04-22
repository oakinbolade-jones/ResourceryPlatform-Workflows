using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class AddTranscriptionDocumentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbNailImage",
                table: "Transcriptions");

            migrationBuilder.AddColumn<byte[]>(
                name: "DocumentData",
                table: "Transcriptions",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentExtension",
                table: "Transcriptions",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentData",
                table: "Transcriptions");

            migrationBuilder.DropColumn(
                name: "DocumentExtension",
                table: "Transcriptions");

            migrationBuilder.AddColumn<string>(
                name: "ThumbNailImage",
                table: "Transcriptions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }
    }
}
