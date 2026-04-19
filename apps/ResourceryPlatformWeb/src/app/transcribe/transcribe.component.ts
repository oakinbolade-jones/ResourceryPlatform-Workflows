import { Component, ElementRef, OnDestroy, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LocalizationService } from '@abp/ng.core';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { Tooltip } from 'bootstrap';
import { ApiErrorLocalizationService } from '../shared/api-error-localization.service';

@Component({
  selector: 'app-transcribe',
  templateUrl: './transcribe.component.html',
  styleUrls: ['./transcribe.component.scss'],
})
export class TranscribeComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('livePreview') livePreviewRef!: ElementRef<HTMLVideoElement>;
  @ViewChild('recordedPlayback') recordedPlaybackRef!: ElementRef<HTMLVideoElement>;

  transcribeForm: FormGroup;
  formatOptions = [
    { value: 'mp4', label: 'MP4' },
    { value: 'mp3', label: 'MP3' },
    { value: 'webm', label: 'WEBM' },
    { value: 'ogg', label: 'OGG' },
    { value: 'mov', label: 'MOV' },
  ];

  isRecording = false;
  isPaused = false;
  isTranscribing = false;
  availableCameras: MediaDeviceInfo[] = [];
  selectedCameraId: string | null = null;
  recordedVideoUrl: string | null = null;
  recordingError: string | null = null;
  recordingInfo: string | null = null;
  saveStatus: string | null = null;
  transcribeStatus: string | null = null;
  stepOneStatus: string | null = null;
  transcriptionPercent = 0;
  transcriptionId: string | null = null;
  transcriptionReferenceId: string | null = null;
  transcriptionResultLinks: { [key: string]: string } | null = null;
  currentStep = 1;
  isSavingStepOne = false;
  isStepOneSaved = false;

  private mediaRecorder: MediaRecorder | null = null;
  private recordedChunks: Blob[] = [];
  private videoStream: MediaStream | null = null;
  private objectUrl: string | null = null;
  private recordedBlob: Blob | null = null;
  private recordedExtension = 'webm';
  private activeMimeType = 'video/webm';
  private statusPollingHandle: ReturnType<typeof setInterval> | null = null;

  // Configure where server-side saving should happen.
  // 1) Update saveEndpoint to your API route.
  // 2) Update saveDirectoryHint to the target folder used by your backend.
  private readonly saveEndpoint = `${environment.apis.default.url}/api/workflow/transcription/save-recording`;
  private readonly saveTranscriptionInfoEndpoint = `${environment.apis.default.url}/api/workflow/transcription/save-info`;
  private readonly transcribeSubmitEndpoint = `${environment.apis.default.url}/api/workflow/transcription/submit-to-wipo`;
  private readonly transcribeStatusEndpoint = `${environment.apis.default.url}/api/workflow/transcription/transcription-status`;
  private readonly downloadResultEndpoint = `${environment.apis.default.url}/api/workflow/transcription/download-result`;
  private readonly saveDirectoryHint = 'D:/RecordedVideos';
  private readonly transcriptionDraftStorageKey = 'workflow.transcription.draft';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private localizationService: LocalizationService,
    private apiErrorLocalization: ApiErrorLocalizationService
  ) {
    this.transcribeForm = this.fb.group({
      Title: ['', Validators.required],
      Description: ['', Validators.required],
      DocumentSetUrl: [''],
      EventDate: [new Date().toISOString().split('T')[0], Validators.required],
      ThumbNailImage: [''],
      TranscriptionMode: ['upload', Validators.required],
      Language: ['en', Validators.required],
      OutputFormat: ['mp4', Validators.required],
      VideoFile: [null],
    });
  }

  get selectedMode(): string {
    return this.transcribeForm.get('TranscriptionMode')?.value ?? 'upload';
  }

  ngOnInit(): void {
    this.restoreStepOneDraft();
    void this.loadCameraDevices();

    const stepOneFields = ['Title', 'Description', 'EventDate', 'Language', 'TranscriptionMode'];
    stepOneFields.forEach(fieldName => {
      this.transcribeForm.get(fieldName)?.valueChanges.subscribe(() => {
        this.persistStepOneDraft();

        if (this.isStepOneSaved) {
          this.isStepOneSaved = false;
          this.stepOneStatus = this.t(
            'Workflow::Transcription:StepOneChanged',
            'Transcription information changed. Please save again to continue.'
          );
        }
      });
    });
  }

  ngAfterViewInit(): void {
    this.initializeTooltips();
  }

  private initializeTooltips(): void {
    const tooltipTriggerList = Array.from(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(tooltipTriggerEl => {
      new Tooltip(tooltipTriggerEl);
    });
  }

  ngOnDestroy(): void {
    this.stopStatusPolling();
    this.stopStream();
    if (this.objectUrl) {
      URL.revokeObjectURL(this.objectUrl);
    }
  }

  onVideoFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files.length > 0 ? input.files[0] : null;
    this.transcribeForm.get('VideoFile')?.setValue(file);
    this.resetTranscriptionState();
  }

  async loadCameraDevices(): Promise<void> {
    try {
      // Request permission first so labels are populated
      await navigator.mediaDevices.getUserMedia({ video: true, audio: true }).then(s => s.getTracks().forEach(t => t.stop()));
      const devices = await navigator.mediaDevices.enumerateDevices();
      this.availableCameras = devices.filter(d => d.kind === 'videoinput');
      if (this.availableCameras.length > 0 && !this.selectedCameraId) {
        this.selectedCameraId = this.availableCameras[0].deviceId;
      }
    } catch {
      // Permissions not yet granted; cameras will be populated when recording starts
    }
  }

  async startRecording(): Promise<void> {
    this.recordingError = null;
    this.recordingInfo = null;
    this.saveStatus = null;
    this.recordedVideoUrl = null;
    this.recordedChunks = [];
    this.recordedBlob = null;
    this.isPaused = false;
    this.transcribeForm.get('VideoFile')?.setValue(null);

    try {
      const videoConstraint: MediaTrackConstraints | boolean = this.selectedCameraId
        ? { deviceId: { exact: this.selectedCameraId } }
        : true;
      this.videoStream = await navigator.mediaDevices.getUserMedia({ video: videoConstraint, audio: true });

      // Refresh device list now that permission is granted
      if (this.availableCameras.length === 0) {
        await this.loadCameraDevices();
      }
    } catch {
      this.recordingError = 'Camera or microphone access was denied.';
      return;
    }

    // Show live preview
    const liveEl = this.livePreviewRef?.nativeElement;
    if (liveEl) {
      liveEl.srcObject = this.videoStream;
      liveEl.muted = true;
      liveEl.play();
    }

    const requestedFormat = this.transcribeForm.get('OutputFormat')?.value ?? 'mp4';
    const resolved = this.resolveRecordingProfile(requestedFormat);
    this.activeMimeType = resolved.mimeType;
    this.recordedExtension = resolved.extension;
    if (resolved.fallbackMessage) {
      this.recordingInfo = resolved.fallbackMessage;
    }

    this.mediaRecorder = new MediaRecorder(this.videoStream, { mimeType: this.activeMimeType });

    this.mediaRecorder.ondataavailable = (event: BlobEvent) => {
      if (event.data.size > 0) {
        this.recordedChunks.push(event.data);
      }
    };

    this.mediaRecorder.onstop = () => {
      const blob = new Blob(this.recordedChunks, { type: this.activeMimeType });
      if (this.objectUrl) {
        URL.revokeObjectURL(this.objectUrl);
      }
      this.objectUrl = URL.createObjectURL(blob);
      this.recordedVideoUrl = this.objectUrl;
      this.recordedBlob = blob;
      this.transcribeForm.get('VideoFile')?.setValue(blob);
      this.stopStream();
    };

    this.mediaRecorder.start(100);
    this.isRecording = true;
  }

  stopRecording(): void {
    if (this.mediaRecorder && this.isRecording) {
      this.mediaRecorder.stop();
      this.isRecording = false;
      this.isPaused = false;
    }
  }

  pauseRecording(): void {
    if (this.mediaRecorder && this.mediaRecorder.state === 'recording' && !this.isPaused) {
      this.mediaRecorder.pause();
      this.isPaused = true;
    }
  }

  resumeRecording(): void {
    if (this.mediaRecorder && this.mediaRecorder.state === 'paused' && this.isPaused) {
      this.mediaRecorder.resume();
      this.isPaused = false;
    }
  }

  saveRecording(): void {
    if (!this.recordedBlob) {
      return;
    }

    this.saveStatus = 'Saving recording to server...';
    const filename = `recording-${new Date().toISOString().replace(/[:.]/g, '-')}.${this.recordedExtension}`;
    const formData = new FormData();
    formData.append('file', this.recordedBlob, filename);
    formData.append('directoryHint', this.saveDirectoryHint);
    formData.append('format', this.recordedExtension);

    fetch(this.saveEndpoint, {
      method: 'POST',
      body: formData,
    })
      .then(async response => {
        if (!response.ok) {
          const message = await this.apiErrorLocalization.resolveMessageFromResponse(
            response,
            'Workflow::Transcription:ApiError:SaveFailed',
            'Unable to save recording at this time.'
          );
          throw new Error(message);
        }
        this.saveStatus = `Saved on server (${this.saveDirectoryHint})`;
      })
      .catch((error: unknown) => {
        const fallbackMessage = this.apiErrorLocalization.resolveNetworkMessage(
          'Workflow::Transcription:ApiError:SaveFailed',
          'Unable to save recording at this time.'
        );
        const message = error instanceof Error && error.message ? error.message : fallbackMessage;
        this.saveStatus = `Save failed: ${message}`;
      });
  }

  async transcribeVideo(): Promise<void> {
    const videoData = this.transcribeForm.get('VideoFile')?.value as Blob | File | null;
    if (!videoData) {
      this.transcribeStatus = 'Please select or record a video first.';
      return;
    }

    this.stopStatusPolling();
    this.isTranscribing = true;
    this.transcriptionPercent = 0;
    this.transcriptionResultLinks = null;

    const sourceReferenceId = this.transcriptionReferenceId ?? crypto.randomUUID();
    this.transcriptionReferenceId = sourceReferenceId;

    const language = this.transcribeForm.get('Language')?.value ?? 'en';
    const inputFormat = this.getInputFormat(videoData);
    const fileName = `transcribe-${sourceReferenceId}.${inputFormat}`;

    const formData = new FormData();
    formData.append('file', videoData, fileName);
    if (this.transcriptionId) {
      formData.append('transcriptionId', this.transcriptionId);
    }
    formData.append('title', this.transcribeForm.get('Title')?.value ?? 'Untitled Transcription');
    const eventDate = this.transcribeForm.get('EventDate')?.value;
    formData.append('dateOfTranscription', eventDate);
    formData.append('eventDate', eventDate);
    formData.append('thumbNailImage', this.transcribeForm.get('ThumbNailImage')?.value ?? '');
    formData.append('inputSource', this.selectedMode === 'record' ? 'Recording' : 'Upload');
    formData.append('sourceReferenceId', sourceReferenceId);
    formData.append('language', language);
    formData.append('inputFormat', inputFormat);

    this.transcribeStatus = 'Submitting video to transcription service...';

    try {
      const response = await fetch(this.transcribeSubmitEndpoint, {
        method: 'POST',
        body: formData,
      });

      if (!response.ok) {
        const message = await this.apiErrorLocalization.resolveMessageFromResponse(
          response,
          'Workflow::Transcription:ApiError:SubmitFailed',
          'Unable to submit media for transcription right now.'
        );
        throw new Error(message);
      }

      const payload = await response.json();
      if (payload?.transcriptionId) {
        this.transcriptionId = String(payload.transcriptionId);
      }

      this.transcribeStatus = 'Submitted. Waiting for transcription progress...';
      this.beginStatusPolling(sourceReferenceId, language);
    } catch (error: unknown) {
      this.isTranscribing = false;
      const fallbackMessage = this.apiErrorLocalization.resolveNetworkMessage(
        'Workflow::Transcription:ApiError:SubmitFailed',
        'Unable to submit media for transcription right now.'
      );
      const message = error instanceof Error && error.message ? error.message : fallbackMessage;
      this.transcribeStatus = `Submit failed: ${message}`;
    }
  }

  private beginStatusPolling(sourceReferenceId: string, language: string): void {
    this.stopStatusPolling();

    const poll = async () => {
      const url = `${this.transcribeStatusEndpoint}?sourceReferenceId=${encodeURIComponent(sourceReferenceId)}&language=${encodeURIComponent(language)}`;

      try {
        const response = await fetch(url);
        if (!response.ok) {
          const message = await this.apiErrorLocalization.resolveMessageFromResponse(
            response,
            'Workflow::Transcription:ApiError:StatusCheckFailed',
            'Unable to check transcription status right now.'
          );
          throw new Error(message);
        }

        const payload = await response.json();
        const first = Array.isArray(payload) && payload.length > 0 ? payload[0] : null;
        if (!first) {
          this.transcribeStatus = 'No transcription status returned yet. Retrying...';
          return;
        }

        const status = String(first.status ?? 'unknown').toLowerCase();
        const percent = Number(first.percent ?? 0);
        this.transcriptionPercent = Number.isFinite(percent) ? percent : 0;
        this.transcribeStatus = `Status: ${status} (${this.transcriptionPercent}%)`;

        if (first.transcript_results) {
          this.transcriptionResultLinks = this.buildResultDownloadLinks(
            first.transcript_results as { [key: string]: string },
            sourceReferenceId,
            language
          );
        }

        if (status === 'finished' || status === 'done' || status === 'completed') {
          this.isTranscribing = false;
          this.stopStatusPolling();
          this.transcribeStatus = `Transcription completed (${this.transcriptionPercent}%).`;
        }

        if (status === 'failed' || status === 'error') {
          this.isTranscribing = false;
          this.stopStatusPolling();
          this.transcribeStatus = 'Transcription failed on remote service.';
        }
      } catch (error: unknown) {
        const fallbackMessage = this.apiErrorLocalization.resolveNetworkMessage(
          'Workflow::Transcription:ApiError:StatusCheckFailed',
          'Unable to check transcription status right now.'
        );
        const message = error instanceof Error && error.message ? error.message : fallbackMessage;
        this.transcribeStatus = `Status poll error: ${message}`;
      }
    };

    void poll();
    this.statusPollingHandle = setInterval(() => {
      void poll();
    }, 10000);
  }

  private stopStatusPolling(): void {
    if (this.statusPollingHandle) {
      clearInterval(this.statusPollingHandle);
      this.statusPollingHandle = null;
    }
  }

  private resetTranscriptionState(): void {
    this.stopStatusPolling();
    this.isTranscribing = false;
    this.transcribeStatus = null;
    this.transcriptionPercent = 0;
    this.transcriptionId = null;
    this.transcriptionReferenceId = null;
    this.transcriptionResultLinks = null;
  }

  private getInputFormat(videoData: Blob | File): string {
    const mode = this.selectedMode;
    if (mode === 'record') {
      return this.recordedExtension;
    }

    const file = videoData as File;
    const dotIndex = file.name.lastIndexOf('.');
    if (dotIndex > -1 && dotIndex < file.name.length - 1) {
      return file.name.substring(dotIndex + 1).toLowerCase();
    }

    return 'mp4';
  }

  downloadRecording(): void {
    if (!this.objectUrl) {
      return;
    }

    const a = document.createElement('a');
    a.href = this.objectUrl;
    a.download = `recording-${new Date().toISOString().replace(/[:.]/g, '-')}.${this.recordedExtension}`;
    a.click();
  }

  private resolveRecordingProfile(requestedFormat: string): {
    mimeType: string;
    extension: string;
    fallbackMessage: string | null;
  } {
    const candidatesByFormat: Record<string, { mimeType: string; extension: string }[]> = {
      mp4: [
        { mimeType: 'video/mp4;codecs=h264,aac', extension: 'mp4' },
        { mimeType: 'video/mp4', extension: 'mp4' },
      ],
      mp3: [
        { mimeType: 'audio/mpeg', extension: 'mp3' },
        { mimeType: 'audio/mp3', extension: 'mp3' },
      ],
      webm: [
        { mimeType: 'video/webm;codecs=vp9,opus', extension: 'webm' },
        { mimeType: 'video/webm;codecs=vp8,opus', extension: 'webm' },
        { mimeType: 'video/webm', extension: 'webm' },
      ],
      ogg: [
        { mimeType: 'video/ogg;codecs=theora,opus', extension: 'ogg' },
        { mimeType: 'video/ogg', extension: 'ogg' },
      ],
      mov: [{ mimeType: 'video/quicktime', extension: 'mov' }],
    };

    const requestedCandidates = candidatesByFormat[requestedFormat] ?? candidatesByFormat['webm'];
    for (const candidate of requestedCandidates) {
      if (MediaRecorder.isTypeSupported(candidate.mimeType)) {
        return { mimeType: candidate.mimeType, extension: candidate.extension, fallbackMessage: null };
      }
    }

    const fallbackCandidates = candidatesByFormat['webm'];
    for (const candidate of fallbackCandidates) {
      if (MediaRecorder.isTypeSupported(candidate.mimeType)) {
        return {
          mimeType: candidate.mimeType,
          extension: candidate.extension,
          fallbackMessage: `${requestedFormat.toUpperCase()} is not supported by this browser. Using ${candidate.extension.toUpperCase()} instead.`,
        };
      }
    }

    return {
      mimeType: 'video/webm',
      extension: 'webm',
      fallbackMessage: `${requestedFormat.toUpperCase()} is not supported by this browser. Using WEBM instead.`,
    };
  }

  private stopStream(): void {
    this.videoStream?.getTracks().forEach(t => t.stop());
    this.videoStream = null;
    const liveEl = this.livePreviewRef?.nativeElement;
    if (liveEl) {
      liveEl.srcObject = null;
    }
  }

  onSubmit(): void {
    if (this.currentStep === 1) {
      void this.saveTranscriptionInfoAndContinue();
      return;
    }

    this.transcribeForm.markAllAsTouched();

    if (this.transcribeForm.invalid) {
      return;
    }

    const value = this.transcribeForm.value;
    console.log('Transcribe submitted:', value);
  }

  async saveTranscriptionInfoAndContinue(): Promise<void> {
    if (this.isStepOneSaved) {
      this.currentStep = 2;
      return;
    }

    this.markStepOneTouched();

    if (!this.isStepOneValid) {
      this.stepOneStatus = this.t(
        'Workflow::Transcription:StepOneInvalid',
        'Please complete all required transcription information before continuing.'
      );
      return;
    }

    this.isSavingStepOne = true;
    this.stepOneStatus = this.t(
      'Workflow::Transcription:StepOneSaving',
      'Saving transcription information...'
    );

    const payload = {
      transcriptionId: this.transcriptionId,
      sourceReferenceId: this.transcriptionReferenceId,
      title: this.transcribeForm.get('Title')?.value,
      description: this.transcribeForm.get('Description')?.value,
      eventDate: this.transcribeForm.get('EventDate')?.value,
      dateOfTranscription: this.transcribeForm.get('EventDate')?.value,
      language: this.transcribeForm.get('Language')?.value,
      transcriptionMode: this.transcribeForm.get('TranscriptionMode')?.value,
      documentSetUrl: this.transcribeForm.get('DocumentSetUrl')?.value ?? '',
      thumbNailImage: this.transcribeForm.get('ThumbNailImage')?.value ?? '',
    };

    try {
      const response = await fetch(this.saveTranscriptionInfoEndpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const message = await this.apiErrorLocalization.resolveMessageFromResponse(
          response,
          'Workflow::Transcription:ApiError:SaveInfoFailed',
          'Unable to save transcription information right now.'
        );
        throw new Error(message);
      }

      const responsePayload = await response.json();
      if (responsePayload?.transcriptionId) {
        this.transcriptionId = String(responsePayload.transcriptionId);
      }
      if (responsePayload?.sourceReferenceId) {
        this.transcriptionReferenceId = String(responsePayload.sourceReferenceId);
      }

      this.persistStepOneDraft(payload, true);
      this.stepOneStatus = this.t(
        'Workflow::Transcription:StepOneSaved',
        'Transcription information saved. Continue with media upload or recording.'
      );
      this.isStepOneSaved = true;
      this.currentStep = 2;
    } catch (error: unknown) {
      // Keep users moving by saving a local draft if API save is unavailable.
      this.persistStepOneDraft(payload, true);
      const fallbackMessage = this.apiErrorLocalization.resolveNetworkMessage(
        'Workflow::Transcription:ApiError:SaveInfoFallback',
        'Server save unavailable. Draft saved locally and moved to step 2.'
      );
      const message = error instanceof Error && error.message ? error.message : fallbackMessage;
      this.stepOneStatus = this.t(
        'Workflow::Transcription:StepOneSavedLocal',
        `Server save unavailable (${message}). Draft saved locally and moved to step 2.`
      );
      this.isStepOneSaved = true;
      this.currentStep = 2;
    } finally {
      this.isSavingStepOne = false;
    }
  }

  goToStep(step: number): void {
    if (step === 1) {
      this.currentStep = 1;
      return;
    }

    if (step === 2 && this.isStepOneValid) {
      this.currentStep = 2;
    }
  }

  goToPaymentPage(): void {
    void this.router.navigate(['/transcription/transcriber-payment'], {
      state: {
        transcriptionId: this.transcriptionId,
        transcriptionReferenceId: this.transcriptionReferenceId,
        title: this.transcribeForm.get('Title')?.value,
      },
    });
  }

  get isStepOneValid(): boolean {
    return (
      !!this.transcribeForm.get('Title')?.valid &&
      !!this.transcribeForm.get('Description')?.valid &&
      !!this.transcribeForm.get('EventDate')?.valid &&
      !!this.transcribeForm.get('Language')?.valid
    );
  }

  private markStepOneTouched(): void {
    this.transcribeForm.get('Title')?.markAsTouched();
    this.transcribeForm.get('Description')?.markAsTouched();
    this.transcribeForm.get('EventDate')?.markAsTouched();
    this.transcribeForm.get('Language')?.markAsTouched();
  }

  private persistStepOneDraft(payload?: any, isSaved?: boolean): void {
    const draft = {
      transcriptionId: this.transcriptionId,
      sourceReferenceId: this.transcriptionReferenceId,
      title: this.transcribeForm.get('Title')?.value,
      description: this.transcribeForm.get('Description')?.value,
      eventDate: this.transcribeForm.get('EventDate')?.value,
      dateOfTranscription: this.transcribeForm.get('EventDate')?.value,
      language: this.transcribeForm.get('Language')?.value,
      transcriptionMode: this.transcribeForm.get('TranscriptionMode')?.value,
      documentSetUrl: this.transcribeForm.get('DocumentSetUrl')?.value ?? '',
      thumbNailImage: this.transcribeForm.get('ThumbNailImage')?.value ?? '',
      isSaved: isSaved ?? this.isStepOneSaved,
      ...payload,
    };

    sessionStorage.setItem(this.transcriptionDraftStorageKey, JSON.stringify(draft));
  }

  private restoreStepOneDraft(): void {
    const raw = sessionStorage.getItem(this.transcriptionDraftStorageKey);
    if (!raw) {
      return;
    }

    try {
      const draft = JSON.parse(raw);
      this.transcribeForm.patchValue({
        Title: draft?.title ?? '',
        Description: draft?.description ?? '',
        EventDate: draft?.eventDate ?? draft?.dateOfTranscription ?? this.transcribeForm.get('EventDate')?.value,
        Language: draft?.language ?? 'en',
        TranscriptionMode: draft?.transcriptionMode ?? 'upload',
        DocumentSetUrl: draft?.documentSetUrl ?? '',
        ThumbNailImage: draft?.thumbNailImage ?? '',
      });

      this.transcriptionId = draft?.transcriptionId ?? null;
      this.transcriptionReferenceId = draft?.sourceReferenceId ?? null;
      this.isStepOneSaved = !!draft?.isSaved;
    } catch {
      sessionStorage.removeItem(this.transcriptionDraftStorageKey);
    }
  }

  private t(key: string, fallback: string): string {
    const value = this.localizationService.instant(key);
    return value && value !== key ? value : fallback;
  }

  private buildResultDownloadLinks(
    transcriptResults: { [key: string]: string },
    sourceReferenceId: string,
    language: string
  ): { [key: string]: string } {
    return Object.keys(transcriptResults).reduce(
      (resultLinks, resultKey) => {
        const upstreamLink = transcriptResults[resultKey];
        if (!upstreamLink) {
          return resultLinks;
        }

        if (this.shouldUseDirectResultLink(resultKey, upstreamLink)) {
          resultLinks[resultKey] = upstreamLink;
          return resultLinks;
        }

        resultLinks[resultKey] =
          `${this.downloadResultEndpoint}?sourceReferenceId=${encodeURIComponent(sourceReferenceId)}` +
          `&language=${encodeURIComponent(language)}` +
          `&resultKey=${encodeURIComponent(resultKey)}`;

        return resultLinks;
      },
      {} as { [key: string]: string }
    );
  }

  private shouldUseDirectResultLink(resultKey: string, upstreamLink: string): boolean {
    const normalizedKey = resultKey.toLowerCase();
    if (normalizedKey.includes('docx')) {
      return false;
    }

    return /^https?:\/\//i.test(upstreamLink);
  }

  goToViewPage(): void {
    if (!this.transcriptionId) return;
    void this.router.navigate(['/transcribe/view-transcription', this.transcriptionId]);
  }

  goToTranscriptionListPage(): void {
    void this.router.navigate(['/transcribe/list']);
  }
}
