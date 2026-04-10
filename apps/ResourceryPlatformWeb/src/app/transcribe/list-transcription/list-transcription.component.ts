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

  constructor(private transcriptionService: TranscriptionService) {}

  ngOnInit(): void {
    this.loadTranscriptions();
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
}
