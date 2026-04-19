import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CreateUpdateTranscriptionDto,
  InputSource,
  TranscriptionDto,
  TranscriptionService,
} from '../../proxy/workflow/transcriptions';

@Component({
  selector: 'app-edit-transcriptions',
  templateUrl: './edit-transcriptions.component.html',
  styleUrls: ['./edit-transcriptions.component.scss'],
})
export class EditTranscriptionsComponent implements OnInit {
  transcriptionId: string | null = null;
  transcription: TranscriptionDto | null = null;
  loading = false;
  saving = false;
  error: string | null = null;
  saveStatus: string | null = null;

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private transcriptionService: TranscriptionService
  ) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      eventDate: ['', Validators.required],
      language: ['en', Validators.required],
      thumbNailImage: [''],
      isPublic: [false],
      status: [''],
    });
  }

  ngOnInit(): void {
    this.transcriptionId = this.route.snapshot.paramMap.get('id');

    if (!this.transcriptionId) {
      this.error = 'Missing transcription id.';
      return;
    }

    this.loadTranscription(this.transcriptionId);
  }

  save(): void {
    if (!this.transcriptionId || this.saving) {
      return;
    }

    this.form.markAllAsTouched();
    if (this.form.invalid || !this.transcription) {
      return;
    }

    const payload = this.buildUpdatePayload(this.transcription, this.form.value);
    this.saving = true;
    this.error = null;
    this.saveStatus = null;

    this.transcriptionService.update(this.transcriptionId, payload).subscribe({
      next: () => {
        this.saving = false;
        this.saveStatus = 'Transcription updated successfully.';
      },
      error: () => {
        this.saving = false;
        this.error = 'Unable to update transcription.';
      },
    });
  }

  goBack(): void {
    void this.router.navigate(['/transcribe/list']);
  }

  private loadTranscription(id: string): void {
    this.loading = true;
    this.error = null;

    this.transcriptionService.get(id).subscribe({
      next: transcription => {
        this.transcription = transcription;
        this.form.patchValue({
          title: transcription.title ?? '',
          description: transcription.description ?? '',
          eventDate: this.toDateInputValue(transcription.eventDate ?? transcription.dateOfTranscription),
          language: transcription.language ?? 'en',
          thumbNailImage: transcription.thumbNailImage ?? '',
          isPublic: !!transcription.isPublic,
          status: transcription.status ?? '',
        });
        this.loading = false;
      },
      error: () => {
        this.error = 'Unable to load transcription.';
        this.loading = false;
      },
    });
  }

  private buildUpdatePayload(
    existing: TranscriptionDto,
    formValue: {
      title: string;
      description: string;
      eventDate: string;
      language: string;
      thumbNailImage: string;
      isPublic: boolean;
      status: string;
    }
  ): CreateUpdateTranscriptionDto {
    return {
      title: formValue.title,
      description: formValue.description,
      isPublic: !!formValue.isPublic,
      dateOfTranscription: formValue.eventDate,
      eventDate: formValue.eventDate,
      mediaFile: existing.mediaFile,
      language: formValue.language,
      inputeFormat: existing.inputeFormat,
      status: formValue.status || existing.status,
      inputSource: existing.inputSource ?? InputSource.Upload,
      thumbNailImage: formValue.thumbNailImage,
      sourceReferenceId: existing.sourceReferenceId,
      linkJson: existing.linkJson,
      linkSrt: existing.linkSrt,
      linkHtml: existing.linkHtml,
      linkTxt: existing.linkTxt,
      linkDocx: existing.linkDocx,
      linkVerbatimDocx: existing.linkVerbatimDocx,
    };
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
