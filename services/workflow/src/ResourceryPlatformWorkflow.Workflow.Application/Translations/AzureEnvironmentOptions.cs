using System;
using Microsoft.Extensions.Configuration;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

internal static class AzureEnvironmentOptions
{
    public static T Resolve<T>(string sectionName, IConfiguration configuration)
        where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        if (section.Exists())
        {
            return section.Get<T>() ?? new T();
        }

        return configuration.GetSection(GetLegacySectionName(sectionName)).Get<T>() ?? new T();
    }

    public static string GetValue(IConfiguration configuration, string sectionName, string propertyName)
    {
        // WebApplication's environment-variables provider maps Azure__Translator__Endpoint
        // to Azure:Translator:Endpoint. Read Environment explicitly too, which is also useful
        // for configuration roots supplied by IIS-hosted and test application modules.
        var environmentName = $"{sectionName.Replace(":", "__")}__{propertyName}";
        var environmentValue = Environment.GetEnvironmentVariable(environmentName);
        if (!string.IsNullOrWhiteSpace(environmentValue))
        {
            return environmentValue;
        }

        var configurationValue = configuration[$"{sectionName}:{propertyName}"];
        if (!IsPlaceholder(configurationValue))
        {
            return configurationValue!;
        }

        var legacySectionName = GetLegacySectionName(sectionName);
        var legacyConfigurationValue = configuration[$"{legacySectionName}:{propertyName}"];
        if (!IsPlaceholder(legacyConfigurationValue))
        {
            return legacyConfigurationValue!;
        }

        var childName = sectionName.Substring(sectionName.LastIndexOf(':') + 1);
        var legacyName = $"AZURE{childName.ToUpperInvariant()}_{propertyName.ToUpperInvariant()}";
        return Environment.GetEnvironmentVariable(legacyName) ??
               (!IsPlaceholder(configurationValue) ? configurationValue : null) ??
               (!IsPlaceholder(legacyConfigurationValue) ? legacyConfigurationValue : null) ??
               string.Empty;
    }

    private static string GetLegacySectionName(string sectionName)
    {
        return sectionName.Replace("Azure:", "Azure", StringComparison.Ordinal);
    }

    private static bool IsPlaceholder(string value)
    {
        return string.IsNullOrWhiteSpace(value) ||
               value.Equals("__SET_VIA_ENVIRONMENT_VARIABLE__", StringComparison.Ordinal);
    }
}
