using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace ResourceryPlatformWorkflow.Administration.SeedData;

public class AdministrationPermissionDataSeedContributor(
    ICurrentTenant currentTenant,
    IPermissionDefinitionManager permissionDefinitionManager,
    IPermissionDataSeeder permissionDataSeeder
) : IDataSeedContributor, ITransientDependency
{
    private const string AdministrationPermissionGroupName = "Administration";
    private const string SettingManagementPermissionGroupName = "SettingManagement";

    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager =
        permissionDefinitionManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder = permissionDataSeeder;

    public async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId.HasValue)
        {
            return;
        }

        using (_currentTenant.Change(context.TenantId))
        {
            var permissions = await _permissionDefinitionManager.GetPermissionsAsync();
            var permissionNames = permissions
                .Where(x =>
                    x.Name.StartsWith(AdministrationPermissionGroupName + ".", StringComparison.Ordinal)
                    || x.Name.StartsWith(SettingManagementPermissionGroupName + ".", StringComparison.Ordinal)
                )
                .Where(x => x.MultiTenancySide.HasFlag(MultiTenancySides.Host))
                .Where(x =>
                    x.Providers.Count == 0
                    || x.Providers.Contains(RolePermissionValueProvider.ProviderName)
                )
                .Select(x => x.Name)
                .ToArray();

            if (permissionNames.Length == 0)
            {
                return;
            }

            await _permissionDataSeeder.SeedAsync(
                RolePermissionValueProvider.ProviderName,
                "admin",
                permissionNames,
                context.TenantId
            );
        }
    }
}
