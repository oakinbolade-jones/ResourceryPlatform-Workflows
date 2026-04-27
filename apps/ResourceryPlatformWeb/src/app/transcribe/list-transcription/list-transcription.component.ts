import { Component, OnInit } from '@angular/core';
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

  constructor(private transcriptionService: TranscriptionService) {}

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
