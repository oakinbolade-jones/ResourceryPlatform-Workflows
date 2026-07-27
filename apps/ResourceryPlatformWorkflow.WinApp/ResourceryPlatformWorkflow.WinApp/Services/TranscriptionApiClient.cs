using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ResourceryPlatformWorkflow.WinApp.Services;

public sealed class TranscriptionApiClient
{
    private readonly HttpClient _httpClient = new();

    public async Task<string> SubmitRecordingAsync(
        Uri submitEndpoint,
        StorageFile recordingFile,
        string sourceReferenceId,
        string title,
        string description,
        string language)
    {
        ArgumentNullException.ThrowIfNull(submitEndpoint);
        ArgumentNullException.ThrowIfNull(recordingFile);

        using var multipart = new MultipartFormDataContent();
        var buffer = await FileIO.ReadBufferAsync(recordingFile);
        using var fileContent = new ByteArrayContent(buffer.ToArray());

        fileContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
        multipart.Add(fileContent, "file", recordingFile.Name);
        multipart.Add(new StringContent(sourceReferenceId ?? string.Empty, Encoding.UTF8), "sourceReferenceId");
        multipart.Add(new StringContent(title ?? string.Empty, Encoding.UTF8), "title");
        multipart.Add(new StringContent(description ?? string.Empty, Encoding.UTF8), "description");
        multipart.Add(new StringContent("false", Encoding.UTF8), "isPublic");
        multipart.Add(new StringContent("false", Encoding.UTF8), "publishedToWebCast");
        multipart.Add(new StringContent(DateTime.UtcNow.ToString("O"), Encoding.UTF8), "dateOfTranscription");
        multipart.Add(new StringContent(DateTime.UtcNow.ToString("O"), Encoding.UTF8), "eventDate");
        multipart.Add(new StringContent("Recording", Encoding.UTF8), "inputSource");
        multipart.Add(new StringContent(string.Empty, Encoding.UTF8), "transcript");
        multipart.Add(new StringContent(string.IsNullOrWhiteSpace(language) ? "en" : language.Trim(), Encoding.UTF8), "language");
        multipart.Add(new StringContent("mp4", Encoding.UTF8), "inputFormat");

        using var response = await _httpClient.PostAsync(submitEndpoint, multipart);
        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Transcription upload failed with status {(int)response.StatusCode}: {responseText}");
        }

        return responseText;
    }
}
