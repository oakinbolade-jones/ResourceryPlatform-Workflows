using System;
using System.Threading;
using System.Threading.Tasks;

namespace ResourceryPlatformWorkflow.Workflow.Translations;


public interface ITranslationService
{
    Task<string> TranslateAsync(
        string text,
        string targetLanguage,
        string? sourceLanguage = null,
        CancellationToken cancellationToken = default);
}