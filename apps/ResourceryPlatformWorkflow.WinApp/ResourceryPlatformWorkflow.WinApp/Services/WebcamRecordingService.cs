using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media;
using Windows.Storage;

namespace ResourceryPlatformWorkflow.WinApp.Services;

public sealed class WebcamRecordingService : IAsyncDisposable
{
    private MediaCapture? _mediaCapture;
    private StorageFile? _currentRecordingFile;
    private bool _isInitialized;
    private bool _isRecording;
    private bool _isPreviewing;

    public bool IsRecording => _isRecording;

    public StorageFile? CurrentRecordingFile => _currentRecordingFile;

    public async Task StartPreviewAsync()
    {
        await EnsureInitializedAsync();
        if (_isPreviewing)
        {
            return;
        }

        await _mediaCapture!.StartPreviewAsync();
        _isPreviewing = true;
    }

    public async Task StopPreviewAsync()
    {
        if (!_isPreviewing || _mediaCapture == null)
        {
            return;
        }

        await _mediaCapture.StopPreviewAsync();
        _isPreviewing = false;
    }

    public async Task<SoftwareBitmap?> CapturePreviewFrameAsync(int width = 640, int height = 360)
    {
        if (_mediaCapture == null)
        {
            return null;
        }

        using var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, width, height);
        await _mediaCapture.GetPreviewFrameAsync(videoFrame);

        if (videoFrame.SoftwareBitmap == null)
        {
            return null;
        }

        using var converted = SoftwareBitmap.Convert(videoFrame.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        return SoftwareBitmap.Copy(converted);
    }

    public async Task<StorageFile> StartRecordingAsync(StorageFolder outputFolder, string filePrefix)
    {
        ArgumentNullException.ThrowIfNull(outputFolder);

        if (_isRecording)
        {
            throw new InvalidOperationException("A recording is already in progress.");
        }

        await EnsureInitializedAsync();

        var safePrefix = SanitizeFileName(string.IsNullOrWhiteSpace(filePrefix) ? "recording" : filePrefix);
        if (string.IsNullOrWhiteSpace(safePrefix))
        {
            safePrefix = "recording";
        }

        var fileName = $"{safePrefix}-{DateTime.Now:yyyyMMdd-HHmmss}.mp4";
        _currentRecordingFile = await outputFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

        var encodingProfile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
        await _mediaCapture!.StartRecordToStorageFileAsync(encodingProfile, _currentRecordingFile);
        _isRecording = true;

        return _currentRecordingFile;
    }

    public async Task StopRecordingAsync()
    {
        if (!_isRecording || _mediaCapture == null)
        {
            return;
        }

        await _mediaCapture.StopRecordAsync();
        _isRecording = false;
    }

    public async ValueTask DisposeAsync()
    {
        if (_mediaCapture == null)
        {
            return;
        }

        if (_isRecording)
        {
            try
            {
                await _mediaCapture.StopRecordAsync();
            }
            catch
            {
                // Ignore cleanup failures during shutdown.
            }
            _isRecording = false;
        }

        if (_isPreviewing)
        {
            try
            {
                await _mediaCapture.StopPreviewAsync();
            }
            catch
            {
                // Ignore cleanup failures during shutdown.
            }
            _isPreviewing = false;
        }

        _mediaCapture.Dispose();
        _mediaCapture = null;
        _isInitialized = false;
    }

    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        _mediaCapture = new MediaCapture();
        var settings = new MediaCaptureInitializationSettings
        {
            StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo
        };

        await _mediaCapture.InitializeAsync(settings);
        _isInitialized = true;
    }

    private static string SanitizeFileName(string value)
    {
        Span<char> buffer = stackalloc char[value.Length];
        var index = 0;

        foreach (var character in value.Trim())
        {
            if (Array.IndexOf(System.IO.Path.GetInvalidFileNameChars(), character) >= 0)
            {
                continue;
            }

            buffer[index++] = character;
        }

        return new string(buffer[..index]).Trim();
    }
}
