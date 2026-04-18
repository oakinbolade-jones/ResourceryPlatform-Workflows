import { Component, ElementRef, Inject, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranscriptionDto, TranscriptionService } from '../../proxy/workflow/transcriptions';
import { DOCUMENT } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-view-transcription',
  templateUrl: './view-transcription.component.html',
  styleUrls: ['./view-transcription.component.scss'],
})
export class ViewTranscriptionComponent implements OnInit, OnDestroy {
  @ViewChild('htmlPreviewFrame') htmlPreviewFrame?: ElementRef<HTMLIFrameElement>;

  transcriptionId: string | null = null;
  transcription: TranscriptionDto | null = null;
  loading = true;
  error: string | null = null;
  linkHtmlUrl: string | null = null;
  linkHtmlFrameUrl: SafeResourceUrl | null = null;

  private readonly transcriptBaseUrl = 'http://4.231.9.50/prdWebroot/ECOWAS';
  private injectedScripts: HTMLScriptElement[] = [];

  constructor(
    private route: ActivatedRoute,
    private renderer: Renderer2,
    @Inject(DOCUMENT) private document: Document,
    private sanitizer: DomSanitizer,
    private transcriptionService: TranscriptionService
  ) {}

  ngOnInit(): void {
    this.transcriptionId = this.route.snapshot.paramMap.get('id') ?? '';

    if (!this.transcriptionId) {
      this.error = 'Missing transcription id.';
      return;
    }

    this.injectTranscriptScripts(this.transcriptionId);
    this.loadTranscription(this.transcriptionId);
  }

  ngOnDestroy(): void {
    this.removeTranscriptScripts();
  }

  private injectTranscriptScripts(id: string): void {
    this.removeTranscriptScripts();

    const stem = `${id}_transcribe-${id}_en_mp4_en`;
    const urls = [
      `${this.transcriptBaseUrl}/${stem}.json.js`,
      `${this.transcriptBaseUrl}/${stem}.json.translation..js`,
    ];

    for (const src of urls) {
      const script = this.renderer.createElement('script') as HTMLScriptElement;
      script.type = 'text/javascript';
      script.src = src;
      script.async = false;
      script.defer = true;
      script.setAttribute('data-transcription-id', id);

      this.renderer.appendChild(this.document.head, script);
      this.injectedScripts.push(script);
    }
  }

  private removeTranscriptScripts(): void {
    for (const s of this.injectedScripts) {
      if (s.parentNode) {
        this.renderer.removeChild(this.document.head, s);
      }
    }
    this.injectedScripts = [];
  }

  private loadTranscription(transcriptionId: string): void {
    if (!this.transcriptionId) {
      this.error = 'Transcription id is missing.';
      this.loading = false;
      return;
    }

    this.loading = true;
    this.error = null;

    this.transcriptionService.get(this.transcriptionId).subscribe({
      next: transcription => {
        this.transcription = transcription;
        this.setHtmlPreviewLink(transcription.linkHtml ?? null);
        this.loading = false;
      },
      error: () => {
        this.error = 'Unable to load transcription.';
        this.transcription = null;
        this.linkHtmlUrl = null;
        this.linkHtmlFrameUrl = null;
        this.loading = false;
      },
    });
  }

  private setHtmlPreviewLink(linkHtml: string | null): void {
    const normalized = (linkHtml ?? '').trim();
    if (!normalized || !/^https?:\/\//i.test(normalized)) {
      this.linkHtmlUrl = null;
      this.linkHtmlFrameUrl = null;
      return;
    }

    const autoplayUrl = this.withAutoplayHints(normalized);
    this.linkHtmlUrl = autoplayUrl;
    this.linkHtmlFrameUrl = this.sanitizer.bypassSecurityTrustResourceUrl(autoplayUrl);
  }

  onHtmlPreviewFrameLoad(): void {
    const iframe = this.htmlPreviewFrame?.nativeElement;
    if (!iframe) {
      return;
    }

    // Best effort only: this works for same-origin iframe documents.
    // For cross-origin pages, browser security blocks direct video control.
    try {
      const iframeDocument = iframe.contentDocument ?? iframe.contentWindow?.document;
      const video = iframeDocument?.querySelector('video') as HTMLVideoElement | null;

      if (!video) {
        return;
      }

      video.muted = true;
      video.playsInline = true;
      void video.play().catch(() => {
        // Ignore autoplay blocking; users can still play manually.
      });
    } catch {
      // Cross-origin iframe access is expected to fail in many cases.
    }
  }

  private withAutoplayHints(url: string): string {
    try {
      const parsed = new URL(url);
      if (!parsed.searchParams.has('autoplay')) {
        parsed.searchParams.set('autoplay', '1');
      }
      if (!parsed.searchParams.has('muted')) {
        parsed.searchParams.set('muted', '1');
      }
      if (!parsed.searchParams.has('playsinline')) {
        parsed.searchParams.set('playsinline', '1');
      }
      return parsed.toString();
    } catch {
      return url;
    }
  }
}
