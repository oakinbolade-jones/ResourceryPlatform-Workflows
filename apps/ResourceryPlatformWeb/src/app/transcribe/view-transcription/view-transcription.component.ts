import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranscriptionDto, TranscriptionService } from '../../proxy/workflow/transcriptions';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

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
  isSearchMatch?: boolean;
  searchMatchIndex?: number;
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

  searchQuery = '';
  searchMatchCount = 0;
  currentMatchIndex = -1;
  downloadError: string | null = null;
  downloadingFormat: 'docx' | 'pdf' | 'txt' | null = null;

  private allWords: TranscriptWord[] = [];
  private playbackFrame: number | null = null;
  private searchMatchPositions: Array<{ lineId: number }> = [];
  private readonly downloadResultEndpoint = `${environment.apis.default.url}/api/workflow/transcription/download-result`;

  constructor(
    private route: ActivatedRoute,
    private transcriptionService: TranscriptionService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.transcriptionId = this.route.snapshot.paramMap.get('id') ?? '';
    const sourceReferenceId = this.route.snapshot.queryParamMap.get('sourceReferenceId') ?? '';

    if (this.transcriptionId && this.transcriptionId !== 'lookup') {
      this.loadTranscriptionById(this.transcriptionId);
      return;
    }

    if (sourceReferenceId) {
      this.loadTranscriptionBySourceReferenceId(sourceReferenceId);
      return;
    }

    this.error = 'Missing transcription identifier.';
    this.loading = false;
  }

  ngOnDestroy(): void {
    this.stopPlaybackTracking();
  }

  private loadTranscriptionById(transcriptionId: string): void {
    if (!transcriptionId) {
      this.error = 'Transcription id is missing.';
      this.loading = false;
      return;
    }

    this.loading = true;
    this.error = null;

    this.transcriptionService.get(transcriptionId).subscribe({
      next: transcription => {
        this.bindLoadedTranscription(transcription);
      },
      error: () => {
        this.handleLoadError('Unable to load transcription.');
      },
    });
  }

  private loadTranscriptionBySourceReferenceId(sourceReferenceId: string): void {
    this.loading = true;
    this.error = null;

    this.transcriptionService.getBySourceReferenceId(sourceReferenceId).subscribe({
      next: transcription => {
        this.transcriptionId = transcription.id ?? this.transcriptionId;
        this.bindLoadedTranscription(transcription);
      },
      error: () => {
        this.handleLoadError('Unable to load transcription.');
      },
    });
  }

  private bindLoadedTranscription(transcription: TranscriptionDto): void {
    this.transcription = transcription;
    this.mediaUrl = this.resolveMediaUrl(transcription);
    this.bindTranscriptFromField(transcription);
    this.loading = false;
  }

  private handleLoadError(message: string): void {
    this.error = message;
    this.transcription = null;
    this.mediaUrl = null;
    this.hasTranscript = false;
    this.transcriptLines = [];
    this.allWords = [];
    this.loading = false;
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

  canDownload(format: 'docx' | 'pdf' | 'txt'): boolean {
    if (!this.transcription) {
      return false;
    }

    const transcriptValue = (this.transcription as unknown as { transcript?: string }).transcript;
    const hasTranscript = !!(transcriptValue && transcriptValue.trim());
    return hasTranscript;
  }

  async downloadResult(format: 'docx' | 'pdf' | 'txt'): Promise<void> {
    if (!this.transcription) {
      return;
    }

    const sourceReferenceId = (this.transcription.sourceReferenceId ?? '').trim();
    const transcriptionId = (this.transcription.id ?? '').trim();
    if (!sourceReferenceId && !transcriptionId) {
      this.downloadError = 'Download is unavailable because transcription identifiers are missing.';
      return;
    }

    const language = (this.transcription.language ?? 'en').trim() || 'en';
    const identifierQuery = sourceReferenceId
      ? `sourceReferenceId=${encodeURIComponent(sourceReferenceId)}`
      : `transcriptionId=${encodeURIComponent(transcriptionId)}`;
    const url =
      `${this.downloadResultEndpoint}?${identifierQuery}` +
      `&language=${encodeURIComponent(language)}` +
      `&resultKey=${encodeURIComponent(format)}`;

    this.downloadError = null;
    this.downloadingFormat = format;

    try {
      const response = await firstValueFrom(
        this.http.get(url, {
          observe: 'response',
          responseType: 'blob',
        })
      );

      const blob = response.body;
      if (!blob) {
        throw new Error('Download failed because the response body was empty.');
      }

      const objectUrl = window.URL.createObjectURL(blob);

      const fileName = this.resolveFileName(response, format);
      const link = document.createElement('a');
      link.href = objectUrl;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(objectUrl);
    } catch (error) {
      this.downloadError =
        error instanceof Error ? error.message : 'Unable to download the selected transcription file.';
    } finally {
      this.downloadingFormat = null;
    }
  }

  private resolveFileName(response: HttpResponse<Blob>, format: 'docx' | 'pdf' | 'txt'): string {
    const contentDisposition = response.headers.get('content-disposition') ?? '';
    const starMatch = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i);
    if (starMatch?.[1]) {
      return decodeURIComponent(starMatch[1]);
    }

    const match = contentDisposition.match(/filename="?([^\";]+)"?/i);
    if (match?.[1]) {
      return match[1];
    }

    const fallbackTitle = (this.transcription?.title ?? 'transcription')
      .trim()
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/^-+|-+$/g, '') || 'transcription';

    return `${fallbackTitle}.${format}`;
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

  onSearchChange(query: string): void {
    this.searchQuery = query;
    this.searchMatchPositions = [];
    this.currentMatchIndex = -1;

    const q = query.trim().toLowerCase();
    if (!q) {
      this.searchMatchCount = 0;
      return;
    }

    for (const line of this.transcriptLines) {
      const text = line.text.toLowerCase();
      let idx = 0;
      while ((idx = text.indexOf(q, idx)) !== -1) {
        this.searchMatchPositions.push({ lineId: line.id });
        idx += q.length;
      }
    }

    this.searchMatchCount = this.searchMatchPositions.length;
    if (this.searchMatchCount > 0) {
      this.currentMatchIndex = 0;
      this.scrollToSearchMatch(0);
    }
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.searchMatchCount = 0;
    this.currentMatchIndex = -1;
    this.searchMatchPositions = [];
  }

  navigateSearch(dir: 1 | -1): void {
    if (!this.searchMatchCount) {
      return;
    }
    this.currentMatchIndex =
      (this.currentMatchIndex + dir + this.searchMatchCount) % this.searchMatchCount;
    this.scrollToSearchMatch(this.currentMatchIndex);
  }

  getDisplaySegments(
    line: TranscriptLine
  ): Array<TranscriptSegment> {
    const q = this.searchQuery.trim().toLowerCase();
    if (!q) {
      return line.segments;
    }

    // Count how many matches exist in lines before this one
    let globalOffset = 0;
    for (const l of this.transcriptLines) {
      if (l.id === line.id) {
        break;
      }
      const t = l.text.toLowerCase();
      let i = 0;
      while ((i = t.indexOf(q, i)) !== -1) {
        globalOffset++;
        i += q.length;
      }
    }

    const result: Array<TranscriptSegment> = [];
    let localMatchIdx = 0;

    for (const seg of line.segments) {
      const lower = seg.text.toLowerCase();
      const parts: Array<TranscriptSegment> = [];
      let cursor = 0;
      let matchPos = lower.indexOf(q, cursor);

      while (matchPos !== -1) {
        if (matchPos > cursor) {
          parts.push({ text: seg.text.slice(cursor, matchPos), word: seg.word });
        }
        parts.push({
          text: seg.text.slice(matchPos, matchPos + q.length),
          word: seg.word,
          isSearchMatch: true,
          searchMatchIndex: globalOffset + localMatchIdx,
        });
        localMatchIdx++;
        cursor = matchPos + q.length;
        matchPos = lower.indexOf(q, cursor);
      }

      if (cursor < seg.text.length) {
        parts.push({ text: seg.text.slice(cursor), word: seg.word });
      }

      result.push(...(parts.length ? parts : [{ text: seg.text, word: seg.word }]));
    }

    return result;
  }

  private scrollToSearchMatch(index: number): void {
    setTimeout(() => {
      const host = this.transcriptScrollHost?.nativeElement;
      if (!host) {
        return;
      }
      const el = host.querySelector(`[data-search-idx="${index}"]`) as HTMLElement | null;
      if (el) {
        el.scrollIntoView({ block: 'center', behavior: 'smooth' });
      }
    }, 0);
  }

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
