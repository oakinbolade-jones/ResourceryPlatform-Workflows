using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class Meetings_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeetingRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ServiceCenterCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayNameItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayNameServiceCenter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayNameItemCategory = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRequirements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NumberOfParticipants = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    HostPhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    HostEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CoHost1Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CoHost1PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CoHost1Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CoHost2Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CoHost2PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CoHost2Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    GLNumberRefreshments = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GLNumberHotel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GLNumberCarHire = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GLNumberEquipment = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GLNumberLanguageServices = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CostCenterNumberRefreshments = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CostCenterNumberHotel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CostCenterNumberCarHire = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CostCenterNumberEquipment = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CostCenterNumberLanguageServices = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeetingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ServiceCenterCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    QuantityNo = table.Column<int>(type: "int", nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemarkObservation = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingItems_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingItems_MeetingId",
                table: "MeetingItems",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingItems_TenantId",
                table: "MeetingItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRequirements_TenantId",
                table: "MeetingRequirements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_ReferenceNumber",
                table: "Meetings",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_TenantId",
                table: "Meetings",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingItems");

            migrationBuilder.DropTable(
                name: "MeetingRequirements");

            migrationBuilder.DropTable(
                name: "Meetings");
        }
    }
}
