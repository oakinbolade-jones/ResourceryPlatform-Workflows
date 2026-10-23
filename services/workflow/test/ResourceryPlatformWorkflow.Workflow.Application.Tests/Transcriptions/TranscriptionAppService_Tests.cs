using System;
using Shouldly;
using Xunit;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public class TranscriptionAppService_Tests
{
    [Fact]
    public void MapForQuery_ShouldOmitDocumentData()
    {
        var transcription = new Transcription(
            Guid.NewGuid(),
            "Quarterly Review",
            "Summary",
            isPublic: true,
            publishedToWebCast: false,
            dateOfTranscription: DateTime.UtcNow,
            eventDate: DateTime.UtcNow,
            mediaFile: "audio.wav",
            language: "en",
            inputeFormat: "wav",
            status: "Completed",
            InputSource.Upload
        );

        transcription.SetDocumentData(new byte[] { 1, 2, 3, 4 });
        transcription.SetSourceReferenceId("123e4567-e89b-12d3-a456-426614174000");
        transcription.SetLinkJson("https://example.com/transcript.json");
        transcription.SetLinkHtml("https://example.com/transcript.html");

        var result = TranscriptionAppService.MapForQuery(transcription);

        result.DocumentData.ShouldBeNull();
        result.SourceReferenceId.ShouldBe("123e4567-e89b-12d3-a456-426614174000");
        result.LinkJson.ShouldBe("https://example.com/transcript.json");
    }
}
