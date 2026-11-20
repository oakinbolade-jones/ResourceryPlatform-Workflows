using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ResourceryPlatformWorkflow.Workflow.Translations;
using Shouldly;
using Xunit;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class AzureEnvironmentOptions_Tests
{
    [Fact]
    public void GetValue_ShouldReadNestedIisEnvironmentVariables()
    {
        var values = new Dictionary<string, string>
        {
            ["AZURE__TRANSLATOR__ENDPOINT"] = "https://translator.test/",
            ["AZURE__TRANSLATOR__KEY1"] = "translator-secret",
            ["AZURE__TRANSLATOR__KEY2"] = "translator-fallback-secret",
            ["AZURE__LANGUAGESERVICES__ENDPOINT"] = "https://language.test/",
            ["AZURE__LANGUAGESERVICES__KEY1"] = "language-secret",
            ["AZURE__LANGUAGESERVICES__KEY2"] = "language-fallback-secret",
            ["AZURE__KEYVAULT__VAULTURI"] = "https://vault.test/"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Azure:Translator:Endpoint"] = "__SET_VIA_ENVIRONMENT_VARIABLE__",
                ["Azure:LanguageService:Endpoint"] = "__SET_VIA_ENVIRONMENT_VARIABLE__",
                ["Azure:KeyVault:VaultUri"] = "__SET_VIA_ENVIRONMENT_VARIABLE__"
            })
            .Build();

        try
        {
            foreach (var value in values)
            {
                Environment.SetEnvironmentVariable(value.Key, value.Value);
            }

            AzureEnvironmentOptions.GetValue(
                configuration,
                AzureTranslatorOptions.SectionName,
                nameof(AzureTranslatorOptions.Endpoint)).ShouldBe("https://translator.test/");
            AzureEnvironmentOptions.GetValue(
                configuration,
                AzureTranslatorOptions.SectionName,
                nameof(AzureTranslatorOptions.Key)).ShouldBe("translator-secret");
            AzureEnvironmentOptions.GetValue(
                configuration,
                AzureLanguageServiceOptions.SectionName,
                nameof(AzureLanguageServiceOptions.Endpoint)).ShouldBe("https://language.test/");
            AzureEnvironmentOptions.GetValue(
                configuration,
                AzureLanguageServiceOptions.SectionName,
                nameof(AzureLanguageServiceOptions.Key1)).ShouldBe("language-secret");
            AzureEnvironmentOptions.GetValue(
                configuration,
                AzureLanguageServiceOptions.SectionName,
                nameof(AzureLanguageServiceOptions.Key2)).ShouldBe("language-fallback-secret");
            AzureEnvironmentOptions.GetValue(
                configuration,
                AzureKeyVaultOptions.SectionName,
                nameof(AzureKeyVaultOptions.VaultUri)).ShouldBe("https://vault.test/");
        }
        finally
        {
            foreach (var name in values.Keys)
            {
                Environment.SetEnvironmentVariable(name, null);
            }
        }
    }
}
