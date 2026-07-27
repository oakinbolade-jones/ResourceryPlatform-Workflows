using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using ResourceryPlatformWorkflow.WinApp.Services;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ResourceryPlatformWorkflow.WinApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const string ApiEndpointSettingKey = "WinApp.TranscriptionApiEndpoint";
        private const string RecordingFolderToken = "WinApp.RecordingFolder";

        private readonly WebcamRecordingService _recordingService = new();
        private readonly TranscriptionApiClient _apiClient = new();
        private readonly SoftwareBitmapSource _previewBitmapSource = new();
        private StorageFolder? _recordingFolder;
        private StorageFile? _lastRecordingFile;
        private string _currentSourceReferenceId = Guid.NewGuid().ToString();
        private DispatcherQueueTimer? _previewTimer;
        private bool _previewFrameInProgress;

        public MainWindow()
        {
            InitializeComponent();
            Closed += (_, _) => _ = OnWindowClosedAsync();

            ApiEndpointTextBox.Text = Environment.GetEnvironmentVariable("RESOURCERY_TRANSCRIPTION_SUBMIT_URL")
                ?? "https://localhost:5001/api/workflow/transcription/submit-to-wipo";
            TitleTextBox.Text = "Desktop recording";
            DescriptionTextBox.Text = "Recorded from Windows desktop app";
            LanguageTextBox.Text = "en";
            CameraPreviewImage.Source = _previewBitmapSource;
            UpdateRecordingFolderText();
            UpdateSourceReferenceText();
            UpdateRecordingFileText();
            UpdateButtons();
            SetStatus("Ready to record.");

            _ = InitializeWindowAsync();
        }

        private async Task InitializeWindowAsync()
        {
            await RestoreSavedSettingsAsync();
            await StartPreviewAsync();
            StartPreviewFramePump();
        }

        private async Task OnWindowClosedAsync()
        {
            StopPreviewFramePump();
            await _recordingService.StopPreviewAsync();
            await _recordingService.DisposeAsync();
        }

        private async Task RestoreSavedSettingsAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue(ApiEndpointSettingKey, out var endpointValue) && endpointValue is string endpoint && !string.IsNullOrWhiteSpace(endpoint))
            {
                ApiEndpointTextBox.Text = endpoint;
            }

            if (StorageApplicationPermissions.FutureAccessList.ContainsItem(RecordingFolderToken))
            {
                try
                {
                    _recordingFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(RecordingFolderToken);
                    UpdateRecordingFolderText();
                }
                catch
                {
                    StorageApplicationPermissions.FutureAccessList.Remove(RecordingFolderToken);
                }
            }
        }

        private async Task StartPreviewAsync()
        {
            try
            {
                await _recordingService.StartPreviewAsync();
                SetStatus("Camera preview ready.");
            }
            catch (Exception ex)
            {
                SetStatus($"Camera preview unavailable: {ex.Message}");
            }
        }

        private void StartPreviewFramePump()
        {
            _previewTimer ??= DispatcherQueue.CreateTimer();
            _previewTimer.Interval = TimeSpan.FromMilliseconds(180);
            _previewTimer.Tick -= PreviewTimer_Tick;
            _previewTimer.Tick += PreviewTimer_Tick;
            _previewTimer.Start();
        }

        private void StopPreviewFramePump()
        {
            if (_previewTimer == null)
            {
                return;
            }

            _previewTimer.Stop();
            _previewTimer.Tick -= PreviewTimer_Tick;
        }

        private void PreviewTimer_Tick(DispatcherQueueTimer sender, object args)
        {
            _ = RefreshPreviewFrameAsync();
        }

        private async Task RefreshPreviewFrameAsync()
        {
            if (_previewFrameInProgress)
            {
                return;
            }

            _previewFrameInProgress = true;
            try
            {
                using var frame = await _recordingService.CapturePreviewFrameAsync();
                if (frame == null)
                {
                    return;
                }

                await _previewBitmapSource.SetBitmapAsync(frame);
            }
            catch
            {
                // Ignore transient preview frame failures.
            }
            finally
            {
                _previewFrameInProgress = false;
            }
        }

        private void ApiEndpointTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveApiEndpointSetting();
        }

        private void SaveApiEndpointSetting()
        {
            var endpoint = ApiEndpointTextBox.Text?.Trim() ?? string.Empty;
            ApplicationData.Current.LocalSettings.Values[ApiEndpointSettingKey] = endpoint;
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            _ = BrowseFolderAsync();
        }

        private async Task BrowseFolderAsync()
        {
            try
            {
                var picker = new FolderPicker();
                picker.FileTypeFilter.Add("*");
                picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));

                var folder = await picker.PickSingleFolderAsync();
                if (folder == null)
                {
                    return;
                }

                _recordingFolder = folder;
                UpdateRecordingFolderText();
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(RecordingFolderToken, folder);
                SetStatus($"Recording folder selected: {folder.Path}");
            }
            catch (Exception ex)
            {
                SetStatus($"Could not choose a recording folder: {ex.Message}");
            }
        }

        private void StartRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            _ = StartRecordingAsync();
        }

        private async Task StartRecordingAsync()
        {
            try
            {
                if (_recordingService.IsRecording)
                {
                    SetStatus("Recording is already in progress.");
                    return;
                }

                if (_recordingFolder == null)
                {
                    await BrowseFolderAsync();
                    if (_recordingFolder == null)
                    {
                        SetStatus("Select a folder before recording.");
                        return;
                    }
                }

                _currentSourceReferenceId = Guid.NewGuid().ToString();
                UpdateSourceReferenceText();
                ApiResponseTextBox.Text = string.Empty;

                _lastRecordingFile = await _recordingService.StartRecordingAsync(_recordingFolder, TitleTextBox.Text);
                UpdateRecordingFileText();
                SetStatus("Recording started. Use Stop Recording when you are done.");
            }
            catch (Exception ex)
            {
                SetStatus($"Recording could not start: {ex.Message}");
            }

            UpdateButtons();
        }

        private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            _ = StopRecordingAsync();
        }

        private async Task StopRecordingAsync()
        {
            try
            {
                await _recordingService.StopRecordingAsync();

                if (_lastRecordingFile != null)
                {
                    UpdateRecordingFileText();
                    SetStatus($"Recording saved locally: {_lastRecordingFile.Path}");
                }
                else
                {
                    SetStatus("Recording stopped.");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Recording could not stop cleanly: {ex.Message}");
            }

            UpdateButtons();
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            _ = UploadRecordingAsync();
        }

        private async Task UploadRecordingAsync()
        {
            try
            {
                if (_lastRecordingFile == null)
                {
                    SetStatus("Record a video first.");
                    return;
                }

                if (!Uri.TryCreate(ApiEndpointTextBox.Text?.Trim(), UriKind.Absolute, out var endpoint))
                {
                    SetStatus("Enter a valid transcription API endpoint.");
                    return;
                }

                SetStatus("Uploading recording to transcription service...");
                var response = await _apiClient.SubmitRecordingAsync(
                    endpoint,
                    _lastRecordingFile,
                    _currentSourceReferenceId,
                    TitleTextBox.Text,
                    DescriptionTextBox.Text,
                    LanguageTextBox.Text
                );

                ApiResponseTextBox.Text = response;
                SetStatus("Upload completed successfully.");
                SaveApiEndpointSetting();
            }
            catch (Exception ex)
            {
                SetStatus($"Upload failed: {ex.Message}");
            }
        }

        private void UpdateButtons()
        {
            StartRecordingButton.IsEnabled = !_recordingService.IsRecording;
            StopRecordingButton.IsEnabled = _recordingService.IsRecording;
            UploadButton.IsEnabled = !_recordingService.IsRecording && _lastRecordingFile != null;
        }

        private void UpdateRecordingFolderText()
        {
            RecordingFolderTextBox.Text = _recordingFolder?.Path ?? string.Empty;
        }

        private void UpdateRecordingFileText()
        {
            RecordingFileTextBlock.Text = _lastRecordingFile?.Path ?? "-";
        }

        private void UpdateSourceReferenceText()
        {
            SourceReferenceTextBlock.Text = $"Source reference: {_currentSourceReferenceId}";
        }

        private void SetStatus(string message)
        {
            StatusTextBlock.Text = message;
        }
    }
}
