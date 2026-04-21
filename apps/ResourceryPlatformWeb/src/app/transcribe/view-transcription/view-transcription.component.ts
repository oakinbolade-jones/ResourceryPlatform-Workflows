import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranscriptionDto, TranscriptionService } from '../../proxy/workflow/transcriptions';

type TranscriptWord = {
  id: number;
  lineId: number;
  text: string;
  start: number;
  end: number;
  startOffset: number;
  endOffset: number;
  localStart: number;
  localEnd: number;
};

type TranscriptSegment = {
  text: string;
  word?: TranscriptWord;
};

type TranscriptLine = {
  id: number;
  text: string;
  startOffset: number;
  endOffset: number;
  words: TranscriptWord[];
  segments: TranscriptSegment[];
};

@Component({
  selector: 'app-view-transcription',
  templateUrl: './view-transcription.component.html',
  styleUrls: ['./view-transcription.component.scss'],
})
export class ViewTranscriptionComponent implements OnInit, OnDestroy {
  @ViewChild('videoPlayer') videoPlayer?: ElementRef<HTMLVideoElement>;
  @ViewChild('transcriptScrollHost') transcriptScrollHost?: ElementRef<HTMLDivElement>;

  transcriptionId: string | null = null;
  transcription: TranscriptionDto | null = null;
  loading = true;
  error: string | null = null;
  mediaUrl: string | null = null;
  transcriptLines: TranscriptLine[] = [];
  hasTranscript = false;
  transcriptParseError: string | null = null;
  activeWordId: number | null = null;
  activeLineId: number | null = null;
  hoveredWordId: number | null = null;
  hoveredLineId: number | null = null;

  private allWords: TranscriptWord[] = [];
  private playbackFrame: number | null = null;

  constructor(
    private route: ActivatedRoute,
    private transcriptionService: TranscriptionService
  ) {}

  ngOnInit(): void {
    this.transcriptionId = this.route.snapshot.paramMap.get('id') ?? '';

    if (!this.transcriptionId) {
      this.error = 'Missing transcription id.';
      this.loading = false;
      return;
    }

    this.loadTranscription(this.transcriptionId);
  }

  ngOnDestroy(): void {
    this.stopPlaybackTracking();
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
        this.mediaUrl = this.resolveMediaUrl(transcription);
        this.bindTranscriptFromField(transcription);
        this.loading = false;
      },
      error: () => {
        this.error = 'Unable to load transcription.';
        this.transcription = null;
        this.mediaUrl = null;
        this.hasTranscript = false;
        this.transcriptLines = [];
        this.allWords = [];
        this.loading = false;
      },
    });
  }

  private resolveMediaUrl(transcription: TranscriptionDto): string | null {
    const linkToVideo = (transcription.linkToVideo ?? '').trim();
    return linkToVideo || null;
  }

  private bindTranscriptFromField(transcription: TranscriptionDto): void {
    this.transcriptParseError = null;
    this.transcriptLines = [];
    this.allWords = [];
    this.activeWordId = null;
    this.activeLineId = null;
    this.hoveredWordId = null;
    this.hoveredLineId = null;

    const rawField = ((transcription as unknown as { transcript?: string }).transcript ?? '').trim();
    if (!rawField) {
      this.hasTranscript = false;
      return;
    }

    const parsed = this.tryParseTranscriptPayload(rawField);
    if (!parsed) {
      this.hasTranscript = false;
      this.transcriptParseError = 'Transcript JSON could not be parsed.';
      return;
    }

    const transcriptText = this.extractTranscriptText(parsed).replace(/\s+$/, '');
    const normalizedWords = this.extractWords(parsed);

    if (!transcriptText) {
      this.hasTranscript = false;
      this.transcriptParseError = 'Transcript JSON is missing transcript text.';
      return;
    }

    this.buildTranscriptLines(transcriptText, normalizedWords);
    this.hasTranscript = this.transcriptLines.length > 0;
  }

  private tryParseTranscriptPayload(rawValue: string): any | null {
    let current: any = rawValue;

    for (let depth = 0; depth < 3; depth++) {
      if (typeof current !== 'string') {
        break;
      }

      const text = current.trim();
      if (!text) {
        return null;
      }

      try {
        current = JSON.parse(text);
      } catch {
        return null;
      }
    }

    return current && typeof current === 'object' ? current : null;
  }

  private extractTranscriptText(payload: any): string {
    if (typeof payload.transcript === 'string') {
      return payload.transcript;
    }

    if (payload.result && typeof payload.result.transcript === 'string') {
      return payload.result.transcript;
    }

    if (Array.isArray(payload.results) && payload.results.length > 0) {
      const first = payload.results[0];
      if (first && typeof first.transcript === 'string') {
        return first.transcript;
      }
    }

    return '';
  }

  private extractWords(payload: any): Array<{ start: number; end: number; startOffset: number; endOffset: number; text: string }> {
    const source =
      (Array.isArray(payload.words) && payload.words) ||
      (payload.result && Array.isArray(payload.result.words) && payload.result.words) ||
      (Array.isArray(payload.word_alignment) && payload.word_alignment) ||
      [];

    const normalized: Array<{ start: number; end: number; startOffset: number; endOffset: number; text: string }> = [];

    for (const item of source) {
      const start = Number(item?.start ?? item?.start_time ?? 0);
      const end = Number(item?.end ?? item?.end_time ?? 0);
      const startOffset = Number(item?.startOffset ?? item?.start_offset ?? -1);
      const endOffset = Number(item?.endOffset ?? item?.end_offset ?? -1);
      const text = String(item?.word ?? item?.text ?? '').trim();

      if (!Number.isFinite(start) || !Number.isFinite(end) || !Number.isFinite(startOffset) || !Number.isFinite(endOffset)) {
        continue;
      }

      if (startOffset < 0 || endOffset <= startOffset) {
        continue;
      }

      normalized.push({ start, end, startOffset, endOffset, text });
    }

    return normalized.sort((a, b) => a.startOffset - b.startOffset);
  }

  private buildTranscriptLines(
    transcriptText: string,
    words: Array<{ start: number; end: number; startOffset: number; endOffset: number; text: string }>
  ): void {
    const rawLines = transcriptText.split('\n');
    let offsetCursor = 0;

    const lines: TranscriptLine[] = rawLines.map((lineText, index) => {
      const lineStart = offsetCursor;
      const lineEnd = lineStart + lineText.length;
      offsetCursor = lineEnd + 1;
      return {
        id: index + 1,
        text: lineText,
        startOffset: lineStart,
        endOffset: lineEnd,
        words: [],
        segments: [],
      };
    });

    const allWords: TranscriptWord[] = [];
    let wordId = 1;

    for (const line of lines) {
      const lineWords = words
        .filter(word => word.startOffset >= line.startOffset && word.endOffset <= line.endOffset)
        .map(word => {
          const localStart = Math.max(0, word.startOffset - line.startOffset);
          const localEnd = Math.min(line.text.length, Math.max(localStart, word.endOffset - line.startOffset));
          const text = line.text.slice(localStart, localEnd) || word.text;
          const mapped: TranscriptWord = {
            id: wordId++,
            lineId: line.id,
            text,
            start: word.start,
            end: word.end,
            startOffset: word.startOffset,
            endOffset: word.endOffset,
            localStart,
            localEnd,
          };
          allWords.push(mapped);
          return mapped;
        });

      line.words = lineWords;
      line.segments = this.buildLineSegments(line.text, lineWords);
    }

    this.transcriptLines = lines;
    this.allWords = allWords.sort((a, b) => a.start - b.start);
  }

  private buildLineSegments(lineText: string, words: TranscriptWord[]): TranscriptSegment[] {
    if (!words.length) {
      return [{ text: lineText }];
    }

    const segments: TranscriptSegment[] = [];
    let cursor = 0;

    for (const word of words) {
      if (word.localStart > cursor) {
        segments.push({ text: lineText.slice(cursor, word.localStart) });
      }

      segments.push({
        text: lineText.slice(word.localStart, word.localEnd),
        word,
      });

      cursor = word.localEnd;
    }

    if (cursor < lineText.length) {
      segments.push({ text: lineText.slice(cursor) });
    }

    return segments;
  }

  onVideoPlay(): void {
    this.startPlaybackTracking();
  }

  onVideoPause(): void {
    this.stopPlaybackTracking();
  }

  onVideoSeeked(): void {
    this.syncToVideoTime();
  }

  onLineHover(lineId: number | null): void {
    this.hoveredLineId = lineId;
  }

  onWordHover(wordId: number | null, lineId: number | null): void {
    this.hoveredWordId = wordId;
    this.hoveredLineId = lineId;
  }

  onWordClick(word: TranscriptWord): void {
    const player = this.videoPlayer?.nativeElement;
    if (!player) {
      return;
    }

    player.currentTime = Math.max(0, word.start || 0);
    void player.play();
    this.syncToVideoTime();
  }

  onLineClick(line: TranscriptLine): void {
    const firstWord = line.words[0];
    if (!firstWord) {
      return;
    }
    this.onWordClick(firstWord);
  }

  isLineActive(lineId: number): boolean {
    return this.activeLineId === lineId;
  }

  isWordActive(wordId: number): boolean {
    return this.activeWordId === wordId;
  }

  trackByLine = (_: number, line: TranscriptLine): number => line.id;

  trackBySegment = (_: number, segment: TranscriptSegment): string =>
    segment.word ? `word-${segment.word.id}` : `txt-${segment.text}`;

  private startPlaybackTracking(): void {
    this.stopPlaybackTracking();

    const step = () => {
      this.syncToVideoTime();
      this.playbackFrame = requestAnimationFrame(step);
    };

    this.playbackFrame = requestAnimationFrame(step);
  }

  private stopPlaybackTracking(): void {
    if (this.playbackFrame !== null) {
      cancelAnimationFrame(this.playbackFrame);
      this.playbackFrame = null;
    }
  }

  private syncToVideoTime(): void {
    const player = this.videoPlayer?.nativeElement;
    if (!player || !this.allWords.length) {
      this.activeWordId = null;
      this.activeLineId = null;
      return;
    }

    const t = player.currentTime;
    const activeWord = this.allWords.find(word => t >= word.start && t <= word.end);

    this.activeWordId = activeWord?.id ?? null;
    this.activeLineId = activeWord?.lineId ?? null;

    if (activeWord) {
      this.scrollActiveWordIntoView(activeWord.id);
    }
  }

  private scrollActiveWordIntoView(wordId: number): void {
    const host = this.transcriptScrollHost?.nativeElement;
    if (!host) {
      return;
    }

    const target = host.querySelector(`[data-word-id="${wordId}"]`) as HTMLElement | null;
    if (!target) {
      return;
    }

    const hostRect = host.getBoundingClientRect();
    const targetRect = target.getBoundingClientRect();
    const thresholdTop = hostRect.top + 40;
    const thresholdBottom = hostRect.bottom - 40;

    if (targetRect.top < thresholdTop || targetRect.bottom > thresholdBottom) {
      target.scrollIntoView({ block: 'center', behavior: 'smooth' });
    }
  }
}
