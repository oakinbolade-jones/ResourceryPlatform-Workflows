import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranscriptionDto, TranscriptionService } from '../../proxy/workflow/transcriptions';

@Component({
  selector: 'app-list-transcription',
  templateUrl: './list-transcription.component.html',
  styleUrls: ['./list-transcription.component.scss'],
})
export class ListTranscriptionComponent implements OnInit {
  loading = false;
  error: string | null = null;
  transcriptions: TranscriptionDto[] = [];
  searchTitle = '';
  searchDate = '';
  deletingId: string | null = null;
  pendingDeleteTranscription: TranscriptionDto | null = null;

  constructor(
    private transcriptionService: TranscriptionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTranscriptions();
  }

  get filteredTranscriptions(): TranscriptionDto[] {
    const titleQuery = this.searchTitle.trim().toLowerCase();
    const dateQuery = this.searchDate;

    return this.transcriptions.filter(item => {
      const title = (item.title ?? '').toLowerCase();
      const titleMatch = !titleQuery || title.includes(titleQuery);
      const dateMatch = !dateQuery || this.toDateInputValue(item.dateOfTranscription) === dateQuery;
      return titleMatch && dateMatch;
    });
  }

  onTitleSearch(value: string): void {
    this.searchTitle = value;
  }

  onDateSearch(value: string): void {
    this.searchDate = value;
  }

  clearFilters(): void {
    this.searchTitle = '';
    this.searchDate = '';
  }

  goToEdit(transcriptionId?: string): void {
    if (!transcriptionId) {
      return;
    }

    void this.router.navigate(['/transcribe/edit', transcriptionId]);
  }

  requestDelete(transcription?: TranscriptionDto): void {
    if (!transcription?.id || this.deletingId) {
      return;
    }

    this.pendingDeleteTranscription = transcription;
  }

  cancelDelete(): void {
    this.pendingDeleteTranscription = null;
  }

  confirmDelete(): void {
    const transcription = this.pendingDeleteTranscription;
    if (!transcription?.id || this.deletingId) {
      return;
    }

    this.deletingId = transcription.id;
    this.error = null;

    this.transcriptionService.delete(transcription.id).subscribe({
      next: () => {
        this.transcriptions = this.transcriptions.filter(item => item.id !== transcription.id);
        this.pendingDeleteTranscription = null;
        this.deletingId = null;
      },
      error: () => {
        this.error = 'Unable to delete transcription.';
        this.deletingId = null;
      },
    });
  }

  private loadTranscriptions(): void {
    this.loading = true;
    this.error = null;

    this.transcriptionService.getList().subscribe({
      next: transcriptions => {
        this.transcriptions = transcriptions;
        this.loading = false;
      },
      error: () => {
        this.error = 'Unable to load transcriptions.';
        this.transcriptions = [];
        this.loading = false;
      },
    });
  }

  private toDateInputValue(value?: string): string {
    if (!value) {
      return '';
    }

    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return '';
    }

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
