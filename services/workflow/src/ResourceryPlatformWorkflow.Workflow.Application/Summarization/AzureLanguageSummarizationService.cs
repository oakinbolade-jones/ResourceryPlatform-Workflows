
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.AI.TextAnalytics;

namespace ResourceryPlatformWorkflow.Workflow.Summarization;

public class AzureLanguageSummarizationService
    : IAzureLanguageSummarizationService
{
    private readonly TextAnalyticsClient _client;

    public AzureLanguageSummarizationService(
        TextAnalyticsClient client)
    {
        _client = client;
    }

    public async Task<string> SummarizeAsync(
        string transcript,
        string language = "en",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(transcript))
        {
            throw new ArgumentException(
                "Transcript cannot be empty.",
                nameof(transcript));
        }

        var documents = new List<string>
        {
            transcript
        };

        var options = new AbstractiveSummarizeOptions
        {
            SentenceCount = 8,

            // Important for confidential institutional transcripts.
            DisableServiceLogs = true
        };

        try
        {
            AbstractiveSummarizeOperation operation =
                await _client.AbstractiveSummarizeAsync(
                    WaitUntil.Completed,
                    documents,
                    language,
                    options,
                    cancellationToken
                );

            var summarySentences = new List<string>();

            await foreach (AbstractiveSummarizeResultCollection documentResult
                in operation.Value)
            {
                foreach (AbstractiveSummarizeResult result in documentResult)
                {
                    if (result.HasError)
                    {
                        throw new InvalidOperationException(
                            $"Azure Language summarization failed: " +
                            $"{result.Error.Message}");
                    }

                    foreach (AbstractiveSummary summary
                        in result.Summaries)
                    {
                        if (!string.IsNullOrWhiteSpace(summary.Text))
                        {
                            summarySentences.Add(summary.Text);
                        }
                    }
                }
            }

            if (summarySentences.Count == 0)
            {
                throw new InvalidOperationException(
                    "Azure Language returned an empty summary.");
            }

            return string.Join(
                Environment.NewLine + Environment.NewLine,
                summarySentences
            );
        }
        catch (RequestFailedException ex)
        {
            throw new InvalidOperationException(
                $"Azure Language request failed. " +
                $"Status: {ex.Status}. " +
                $"Error: {ex.Message}",
                ex
            );
        }
    }
}
