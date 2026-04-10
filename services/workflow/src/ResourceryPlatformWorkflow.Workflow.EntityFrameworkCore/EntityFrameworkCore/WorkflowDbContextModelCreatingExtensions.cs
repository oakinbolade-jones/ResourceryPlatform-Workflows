using System;
using Microsoft.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Workflow.Meetings;
using ResourceryPlatformWorkflow.Workflow.Requests;
using ResourceryPlatformWorkflow.Workflow.Services;
using ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;
using ResourceryPlatformWorkflow.Workflow.Transcriptions;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;

public static class WorkflowDbContextModelCreatingExtensions
{
    public static void ConfigureWorkflow(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Request>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "Requests", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.DocumentSetUrl)
                .HasMaxLength(RequestConsts.MaxDocumentSetUrlLength);
            b.Property(x => x.Description)
                .HasMaxLength(RequestConsts.MaxRequestDescriptionLength);
            b.Property(x => x.Comment)
                .HasMaxLength(RequestConsts.MaxCommentLength);
            b.Property(x => x.ServiceId).IsRequired();
            b.Property(x => x.RequestType).IsRequired();
            b.Property(x => x.RequestStatus).IsRequired();
            b.Property(x => x.DocumentMigrationStatus).IsRequired();
            b.Property(x => x.DocumentsPublishedAt);

            b.HasMany(x => x.Documents)
                .WithOne()
                .HasForeignKey(x => x.RequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.MeetingForm)
                .WithOne(x => x.Request)
                .HasForeignKey<Meeting>(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<RequestDocument>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "Documents", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(RequestConsts.MaxDocumentTitleLength);
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(RequestConsts.MaxDocumentDescriptionLength);
            b.Property(x => x.DocumentUrl)
                .IsRequired()
                .HasMaxLength(RequestConsts.MaxDocumentUrlLength);
            b.Property(x => x.SharePointDocumentUrl)
                .HasMaxLength(RequestConsts.MaxDocumentUrlLength);
            b.Property(x => x.SharePointItemId)
                .HasMaxLength(RequestConsts.MaxSharePointItemIdLength);
            b.Property(x => x.MigrationStatus).IsRequired();
            b.Property(x => x.LastMigrationError)
                .HasMaxLength(RequestConsts.MaxMigrationErrorLength);
            b.Property(x => x.MigratedAt);

            b.HasIndex(x => x.RequestId);
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<Service>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "Services", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.ServiceCenterId).IsRequired();
            b.Property(x => x.Name).IsRequired().HasMaxLength(ServiceConsts.MaxServiceNameLength);
            b.Property(x => x.Code).IsRequired().HasMaxLength(ServiceConsts.MaxServiceCodeLength);
            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(ServiceConsts.MaxServiceDisplayNameLength);
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(ServiceConsts.MaxServiceDescriptionLength);
            b.Property(x => x.IsActive).IsRequired();

            b.HasIndex(x => x.Name).IsUnique();
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.ServiceCenterId);
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<ServiceCenter>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "ServiceCenters", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(ServiceCenterConsts.MaxServiceCenterNameLength);
            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(ServiceCenterConsts.MaxServiceCenterDisplayNameLength);
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(ServiceCenterConsts.MaxServiceCenterDescriptionLength);
            b.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(ServiceCenterConsts.MaxServiceCenterCodeLength);

            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<ServiceWorkflow>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "ServiceWorkflows", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxWorkflowNameLength);
            b.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxWorkflowCodeLength);
            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxWorkflowDisplayNameLength); b.Property(x => x.LeadTime)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxLeadTimeLength);
            b.Property(x => x.LeadTimeType)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxLeadTimeTypeLength); b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxWorkflowDescriptionLength);
            b.Property(x => x.IsActive).IsRequired();

            b.HasMany(x => x.Steps)
                .WithOne()
                .HasForeignKey(x => x.ServiceWorkflowId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<ServiceWorkflowStep>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "ServiceWorkflowSteps", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepNameLength);
            b.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepCodeLength);
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepDescriptionLength);
            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepDisplayNameProcessLength);
            b.Property(x => x.DisplayNameOutput)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepDisplayNameExpectedOutputLength);
            b.Property(x => x.Output)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepOutputLength);
            b.Property(x => x.TATType)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepTATTypeLength);
            b.Property(x => x.TATUnit)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepTATUnitLength);
            b.Property(x => x.Order).IsRequired();

            b.HasIndex(x => x.ServiceWorkflowId);
            b.HasIndex(x => new { x.ServiceWorkflowId, x.Order }).IsUnique();
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<ServiceWorkflowInstance>(b =>
        {
            b.ToTable(
                WorkflowDbProperties.DbTablePrefix + "ServiceWorkflowInstances",
                WorkflowDbProperties.DbSchema
            );

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.ServiceWorkflowId).IsRequired();
            b.Property(x => x.RequestId).IsRequired();
            b.Property(x => x.CurrentStepId);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.StartedAt).IsRequired();
            b.Property(x => x.CompletedAt);

            b.HasMany(x => x.Tasks)
                .WithOne()
                .HasForeignKey(x => x.ServiceWorkflowInstanceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.HistoryEntries)
                .WithOne()
                .HasForeignKey(x => x.ServiceWorkflowInstanceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.ServiceWorkflowId);
            b.HasIndex(x => x.RequestId).IsUnique();
            b.HasIndex(x => x.CurrentStepId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<ServiceWorkflowTask>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "ServiceWorkflowTasks", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.ServiceWorkflowInstanceId).IsRequired();
            b.Property(x => x.ServiceWorkflowStepId).IsRequired();
            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxTaskTitleLength);
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxTaskDescriptionLength);
            b.Property(x => x.AssigneeUserId);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.DueDate);
            b.Property(x => x.CompletedAt);

            b.HasIndex(x => x.ServiceWorkflowInstanceId);
            b.HasIndex(x => x.ServiceWorkflowStepId);
            b.HasIndex(x => x.AssigneeUserId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<ServiceWorkflowHistory>(b =>
        {
            b.ToTable(
                WorkflowDbProperties.DbTablePrefix + "ServiceWorkflowHistory",
                WorkflowDbProperties.DbSchema
            );

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.ServiceWorkflowInstanceId).IsRequired();
            b.Property(x => x.ServiceWorkflowStepId);
            b.Property(x => x.ServiceWorkflowTaskId);
            b.Property(x => x.Type).IsRequired();
            b.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxHistoryActionLength);
            b.Property(x => x.Comment)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxHistoryCommentLength);
            b.Property(x => x.PerformedByUserId);
            b.Property(x => x.PerformedAt).IsRequired();

            b.HasIndex(x => x.ServiceWorkflowInstanceId);
            b.HasIndex(x => x.ServiceWorkflowStepId);
            b.HasIndex(x => x.ServiceWorkflowTaskId);
            b.HasIndex(x => x.Type);
            b.HasIndex(x => x.PerformedAt);
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<Meeting>(b =>
           {
               b.ToTable(WorkflowDbProperties.DbTablePrefix + "Meetings", WorkflowDbProperties.DbSchema);

               b.ConfigureByConvention();
               b.ConfigureMultiTenant();

               b.Property(x => x.RequestId);
               b.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(MeetingConsts.MaxTitleLength);
               b.Property(x => x.ReferenceNumber)
                   .IsRequired()
                   .HasMaxLength(MeetingConsts.MaxReferenceNumberLength);
               b.Property(x => x.Location)
                   .IsRequired()
                   .HasMaxLength(MeetingConsts.MaxLocationLength);
               b.Property(x => x.ContactPhone)
                   .HasMaxLength(MeetingConsts.MaxContactPhoneLength);
               b.Property(x => x.ContactEmail)
                   .HasMaxLength(MeetingConsts.MaxContactEmailLength);
               b.Property(x => x.ContactName)
                   .HasMaxLength(MeetingConsts.MaxContactNameLength);
               b.Property(x => x.HostName)
                   .HasMaxLength(MeetingConsts.MaxHostNameLength);
               b.Property(x => x.HostPhoneNumber)
                   .HasMaxLength(MeetingConsts.MaxHostPhoneNumberLength);
               b.Property(x => x.HostEmail)
                   .HasMaxLength(MeetingConsts.MaxHostEmailLength);
               b.Property(x => x.CoHost1Name)
                   .HasMaxLength(MeetingConsts.MaxCoHostNameLength);
               b.Property(x => x.CoHost1PhoneNumber)
                   .HasMaxLength(MeetingConsts.MaxCoHostPhoneNumberLength);
               b.Property(x => x.CoHost1Email)
                   .HasMaxLength(MeetingConsts.MaxCoHostEmailLength);
               b.Property(x => x.CoHost2Name)
                   .HasMaxLength(MeetingConsts.MaxCoHostNameLength);
               b.Property(x => x.CoHost2PhoneNumber)
                   .HasMaxLength(MeetingConsts.MaxCoHostPhoneNumberLength);
               b.Property(x => x.CoHost2Email)
                   .HasMaxLength(MeetingConsts.MaxCoHostEmailLength);
               b.Property(x => x.GLNumberRefreshments)
                   .HasMaxLength(MeetingConsts.MaxGLNumberLength);
               b.Property(x => x.GLNumberHotel)
                   .HasMaxLength(MeetingConsts.MaxGLNumberLength);
               b.Property(x => x.GLNumberCarHire)
                   .HasMaxLength(MeetingConsts.MaxGLNumberLength);
               b.Property(x => x.GLNumberEquipment)
                   .HasMaxLength(MeetingConsts.MaxGLNumberLength);
               b.Property(x => x.GLNumberLanguageServices)
                   .HasMaxLength(MeetingConsts.MaxGLNumberLength);
               b.Property(x => x.CostCenterNumberRefreshments)
                   .HasMaxLength(MeetingConsts.MaxCostCenterNumberLength);
               b.Property(x => x.CostCenterNumberHotel)
                   .HasMaxLength(MeetingConsts.MaxCostCenterNumberLength);
               b.Property(x => x.CostCenterNumberCarHire)
                   .HasMaxLength(MeetingConsts.MaxCostCenterNumberLength);
               b.Property(x => x.CostCenterNumberEquipment)
                   .HasMaxLength(MeetingConsts.MaxCostCenterNumberLength);
               b.Property(x => x.CostCenterNumberLanguageServices)
                   .HasMaxLength(MeetingConsts.MaxCostCenterNumberLength);

               b.HasMany(x => x.MeetingItems)
                   .WithOne(x => x.Meeting)
                   .HasForeignKey(x => x.MeetingId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

               b.HasIndex(x => x.TenantId);
               b.HasIndex(x => x.ReferenceNumber).IsUnique();
               b.HasIndex(x => x.RequestId)
                   .IsUnique()
                   .HasFilter("[RequestId] IS NOT NULL");
           });

        builder.Entity<MeetingItem>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "MeetingItems", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.ItemName)
                .IsRequired()
                .HasMaxLength(MeetingItemConsts.MaxItemNameLength);
            b.Property(x => x.ItemCode)
                .IsRequired()
                .HasMaxLength(MeetingItemConsts.MaxItemCodeLength);
            b.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(MeetingItemConsts.MaxCategoryLength);
            b.Property(x => x.ServiceCenterCode)
                .IsRequired()
                .HasMaxLength(MeetingItemConsts.MaxServiceCenterCodeLength);
            b.Property(x => x.RemarkObservation)
                .HasMaxLength(MeetingItemConsts.MaxRemarkObservationLength);
            b.Property(x => x.Budget)
                .IsRequired()
                .HasPrecision(18, 2);
            b.HasOne(x => x.Meeting)
                .WithMany(x => x.MeetingItems)
                .HasForeignKey(x => x.MeetingId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.MeetingId);
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<MeetingRequirement>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "MeetingRequirements", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.ItemName)
                .IsRequired()
                .HasMaxLength(MeetingRequirementConsts.MaxItemNameLength);
            b.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(MeetingRequirementConsts.MaxCategoryLength);
            b.Property(x => x.ServiceCenterCode)
                .IsRequired()
                .HasMaxLength(MeetingRequirementConsts.MaxServiceCenterCodeLength);
            b.Property(x => x.DisplayNameItemName)
                .HasMaxLength(MeetingRequirementConsts.MaxDisplayNameLength);
            b.Property(x => x.DisplayNameServiceCenter)
                .HasMaxLength(MeetingRequirementConsts.MaxDisplayNameLength);
            b.Property(x => x.DisplayNameItemCategory)
                .HasMaxLength(MeetingRequirementConsts.MaxDisplayNameLength);

            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<Transcription>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "Transcriptions", WorkflowDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureMultiTenant();

            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(TranscriptionConsts.MaxTitleLength);
            b.Property(x => x.Description)
                .HasMaxLength(TranscriptionConsts.MaxDescriptionLength);
            b.Property(x => x.IsPublic).IsRequired();
            b.Property(x => x.DateOfTranscription).IsRequired();
            b.Property(x => x.EventDate);
            b.Property(x => x.MediaFile)
                .HasMaxLength(TranscriptionConsts.MaxMediaFileLength);
            b.Property(x => x.Language)
                .IsRequired()
                .HasMaxLength(TranscriptionConsts.MaxLanguageLength);
            b.Property(x => x.InputeFormat)
                .IsRequired()
                .HasMaxLength(TranscriptionConsts.MaxInputeFormatLength);
            b.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(TranscriptionConsts.MaxStatusLength);
            b.Property(x => x.InputSource).IsRequired();
            b.Property(x => x.ThumbNailImage)
                .HasMaxLength(TranscriptionConsts.MaxThumbNailImageLength);
            b.Property(x => x.SourceReferenceId)
                .HasMaxLength(TranscriptionConsts.MaxSourceReferenceIdLength);
            b.Property(x => x.LinkJson)
                .HasMaxLength(TranscriptionConsts.MaxResultLinkLength);
            b.Property(x => x.LinkSrt)
                .HasMaxLength(TranscriptionConsts.MaxResultLinkLength);
            b.Property(x => x.LinkHtml)
                .HasMaxLength(TranscriptionConsts.MaxResultLinkLength);
            b.Property(x => x.LinkTxt)
                .HasMaxLength(TranscriptionConsts.MaxResultLinkLength);
            b.Property(x => x.LinkDocx)
                .HasMaxLength(TranscriptionConsts.MaxResultLinkLength);
            b.Property(x => x.LinkVerbatimDocx)
                .HasMaxLength(TranscriptionConsts.MaxResultLinkLength);

            b.HasIndex(x => x.SourceReferenceId);
            b.HasIndex(x => x.TenantId);
        });



    }






}
