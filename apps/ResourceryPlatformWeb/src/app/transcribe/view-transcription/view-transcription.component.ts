import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranscriptionDto, TranscriptionService } from '../../proxy/workflow/transcriptions';

@Component({
  selector: 'app-view-transcription',
  templateUrl: './view-transcription.component.html',
  styleUrls: ['./view-transcription.component.scss'],
})
export class ViewTranscriptionComponent implements OnInit {
  transcriptionId: string | null = null;
  transcription: TranscriptionDto | null = null;
  loading = true;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private transcriptionService: TranscriptionService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.transcriptionId = params.get('id');
      if (this.transcriptionId) {
        this.loadTranscription();
      } else {
        this.error = 'Transcription id is missing.';
        this.loading = false;
      }
    });
  }

  private loadTranscription(): void {
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
        this.loading = false;
      },
      error: () => {
        this.error = 'Unable to load transcription.';
        this.transcription = null;
        this.loading = false;
      },
    });
  }
}
