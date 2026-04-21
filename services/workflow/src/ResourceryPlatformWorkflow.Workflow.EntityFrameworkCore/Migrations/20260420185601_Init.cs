using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceryPlatformWorkflow.Workflow.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
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
                    DisplayNameItemName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayNameServiceCenter = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayNameItemCategory = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRequirements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    DocumentSetUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentMigrationStatus = table.Column<int>(type: "int", nullable: false),
                    DocumentsPublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceWorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_ServiceWorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    LeadTime = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LeadTimeType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ServiceWorkflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transcriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    PublishedToWebCast = table.Column<bool>(type: "bit", nullable: false),
                    DateOfTranscription = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MediaFile = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    InputeFormat = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    InputSource = table.Column<int>(type: "int", nullable: false),
                    ThumbNailImage = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    SourceReferenceId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Transcript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkJson = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LinkSrt = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LinkHtml = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LinkTxt = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LinkDocx = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LinkVerbatimDocx = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
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
                    table.PrimaryKey("PK_Transcriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    SharePointDocumentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    SharePointItemId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MigrationStatus = table.Column<int>(type: "int", nullable: false),
                    LastMigrationError = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    MigratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NumberOfParticipants = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HostName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HostDesignation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HostPhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    HostEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CoHost1Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CoHost1Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoHost1PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CoHost1Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CoHost2Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CoHost2Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Meetings_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflowHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceWorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceWorkflowStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceWorkflowTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_ServiceWorkflowHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceWorkflowHistory_ServiceWorkflowInstances_ServiceWorkflowInstanceId",
                        column: x => x.ServiceWorkflowInstanceId,
                        principalTable: "ServiceWorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflowTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceWorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceWorkflowStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    AssigneeUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_ServiceWorkflowTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceWorkflowTasks_ServiceWorkflowInstances_ServiceWorkflowInstanceId",
                        column: x => x.ServiceWorkflowInstanceId,
                        principalTable: "ServiceWorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceWorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    DisplayNameOutput = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Output = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    TATType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TATUnit = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ServiceWorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceWorkflowSteps_ServiceWorkflows_ServiceWorkflowId",
                        column: x => x.ServiceWorkflowId,
                        principalTable: "ServiceWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                name: "IX_Documents_RequestId",
                table: "Documents",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TenantId",
                table: "Documents",
                column: "TenantId");

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
                name: "IX_Meetings_RequestId",
                table: "Meetings",
                column: "RequestId",
                unique: true,
                filter: "[RequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_TenantId",
                table: "Meetings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TenantId",
                table: "Requests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCenters_Code",
                table: "ServiceCenters",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCenters_TenantId",
                table: "ServiceCenters",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Code",
                table: "Services",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_Name",
                table: "Services",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCenterId",
                table: "Services",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_TenantId",
                table: "Services",
                column: "TenantId");

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
                name: "IX_ServiceWorkflowHistory_TenantId",
                table: "ServiceWorkflowHistory",
                column: "TenantId");

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
                name: "IX_ServiceWorkflowInstances_TenantId",
                table: "ServiceWorkflowInstances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflows_Code",
                table: "ServiceWorkflows",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflows_Name",
                table: "ServiceWorkflows",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflows_TenantId",
                table: "ServiceWorkflows",
                column: "TenantId");

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
                name: "IX_ServiceWorkflowSteps_TenantId",
                table: "ServiceWorkflowSteps",
                column: "TenantId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWorkflowTasks_TenantId",
                table: "ServiceWorkflowTasks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Transcriptions_SourceReferenceId",
                table: "Transcriptions",
                column: "SourceReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Transcriptions_TenantId",
                table: "Transcriptions",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "MeetingItems");

            migrationBuilder.DropTable(
                name: "MeetingRequirements");

            migrationBuilder.DropTable(
                name: "ServiceCenters");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "ServiceWorkflowHistory");

            migrationBuilder.DropTable(
                name: "ServiceWorkflowSteps");

            migrationBuilder.DropTable(
                name: "ServiceWorkflowTasks");

            migrationBuilder.DropTable(
                name: "Transcriptions");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "ServiceWorkflows");

            migrationBuilder.DropTable(
                name: "ServiceWorkflowInstances");

            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
