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
        // The canonical environment-variable names use uppercase sections separated by double underscores,
        // for example AZURE__TRANSLATOR__KEY, so they also work on case-sensitive hosts.
        var environmentName = $"{sectionName.Replace(":", "__")}__{propertyName}".ToUpperInvariant();
        if (sectionName.Equals(AzureLanguageServiceOptions.SectionName, StringComparison.OrdinalIgnoreCase) &&
            propertyName.Equals(nameof(AzureLanguageServiceOptions.Endpoint), StringComparison.OrdinalIgnoreCase))
        {
            environmentName = "AZURE__LANGUAGESERVICES__ENDPOINT";
        }

        var environmentValue = Environment.GetEnvironmentVariable(environmentName);
        if (sectionName.Equals(AzureTranslatorOptions.SectionName, StringComparison.OrdinalIgnoreCase) &&
            propertyName.Equals(nameof(AzureTranslatorOptions.Key), StringComparison.OrdinalIgnoreCase))
        {
            environmentValue = FirstNonEmpty(
                Environment.GetEnvironmentVariable("AZURE__TRANSLATOR__KEY1"),
                Environment.GetEnvironmentVariable("AZURE__TRANSLATOR__KEY2")
            );
        }
        else if (sectionName.Equals(AzureLanguageServiceOptions.SectionName, StringComparison.OrdinalIgnoreCase) &&
                 propertyName.Equals(nameof(AzureLanguageServiceOptions.Key), StringComparison.OrdinalIgnoreCase))
        {
            environmentValue = FirstNonEmpty(
                Environment.GetEnvironmentVariable("AZURE__LANGUAGESERVICES__KEY1"),
                Environment.GetEnvironmentVariable("AZURE__LANGUAGESERVICES__KEY2")
            );
        }
        else if (sectionName.Equals(AzureLanguageServiceOptions.SectionName, StringComparison.OrdinalIgnoreCase) &&
                 propertyName.Equals(nameof(AzureLanguageServiceOptions.Key1), StringComparison.OrdinalIgnoreCase))
        {
            environmentValue = Environment.GetEnvironmentVariable("AZURE__LANGUAGESERVICES__KEY1");
        }
        else if (sectionName.Equals(AzureLanguageServiceOptions.SectionName, StringComparison.OrdinalIgnoreCase) &&
                 propertyName.Equals(nameof(AzureLanguageServiceOptions.Key2), StringComparison.OrdinalIgnoreCase))
        {
            environmentValue = Environment.GetEnvironmentVariable("AZURE__LANGUAGESERVICES__KEY2");
        }

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

        return (!IsPlaceholder(configurationValue) ? configurationValue : null) ??
               (!IsPlaceholder(legacyConfigurationValue) ? legacyConfigurationValue : null) ??
               string.Empty;
    }

    private static string GetLegacySectionName(string sectionName)
    {
        return sectionName.Replace("Azure:", "Azure", StringComparison.Ordinal);
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static bool IsPlaceholder(string value)
    {
        return string.IsNullOrWhiteSpace(value) ||
               value.Equals("__SET_VIA_ENVIRONMENT_VARIABLE__", StringComparison.Ordinal);
    }
}
