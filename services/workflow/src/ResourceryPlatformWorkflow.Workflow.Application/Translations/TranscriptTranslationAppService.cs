using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.AI.Translation.Text;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class TranscriptTranslationAppService : ApplicationService
{
    private readonly TextTranslationClient _client;

    public TranscriptTranslationAppService(TextTranslationClient client)
    {
        _client = client;
    }

    public async Task<string> TranslateAsync(
        string text,
        string targetLanguage,
        string? sourceLanguage = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var response = await _client.TranslateAsync(
            targetLanguage,
            text,
            sourceLanguage,
            cancellationToken: cancellationToken
        );

        var translation = response.Value.FirstOrDefault()
            ?.Translations?.FirstOrDefault()?.Text;

        return translation ?? string.Empty;
    }

}