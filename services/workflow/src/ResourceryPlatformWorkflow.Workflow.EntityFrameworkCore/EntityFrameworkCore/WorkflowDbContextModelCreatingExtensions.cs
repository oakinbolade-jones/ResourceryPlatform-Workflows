using System;
using Microsoft.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Workflow.Requests;
using ResourceryPlatformWorkflow.Workflow.Services;
using ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;
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
                .IsRequired()
                .HasMaxLength(RequestConsts.MaxDocumentSetUrlLength);
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(RequestConsts.MaxRequestDescriptionLength);
            b.Property(x => x.RequestType).IsRequired();
            b.Property(x => x.RequestStatus).IsRequired();
            b.Property(x => x.DocumentMigrationStatus).IsRequired();
            b.Property(x => x.DocumentsPublishedAt);

            b.HasMany(x => x.Documents)
                .WithOne()
                .HasForeignKey(x => x.RequestId)
                .IsRequired()
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
            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(ServiceConsts.MaxServiceDisplayNameLength);
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(ServiceConsts.MaxServiceDescriptionLength);
            b.Property(x => x.IsActive).IsRequired();

            b.HasIndex(x => x.Name).IsUnique();
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
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxWorkflowDescriptionLength);
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.ServiceId).HasColumnName("RelationServiceId");
            b.HasIndex(x => x.ServiceId);

            b.HasMany(x => x.Steps)
                .WithOne()
                .HasForeignKey(x => x.ServiceWorkflowId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.Name).IsUnique();
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
            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(ServiceWorkflowConsts.MaxStepDescriptionLength);
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
    }
}
