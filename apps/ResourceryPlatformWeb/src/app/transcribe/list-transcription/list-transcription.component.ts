import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { LocalizationService, PermissionService } from '@abp/ng.core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranscriptionDto, TranscriptionService } from '../../proxy/workflow/transcriptions';
@Component({
  selector: 'app-list-transcription',
  templateUrl: './list-transcription.component.html',
  styleUrls: ['./list-transcription.component.scss'],
})
export class ListTranscriptionComponent implements OnInit {
  @ViewChild('deleteTranscriptionModal') deleteTranscriptionModal!: TemplateRef<unknown>;

  loading = false;
  error: string | null = null;
  deletingId: string | null = null;
  transcriptions: TranscriptionDto[] = [];
  searchTitle = '';
  searchDate = '';
  hasTranscriptionCreatePermission = false;
  hasTranscriptionViewPermission = false;
  hasTranscriptionUpdatePermission = false;
  hasTranscriptionDeletePermission = false;
  pendingDeleteItem: TranscriptionDto | null = null;
  deleteConfirmationTitle = '';
  actionLabel = 'Action';
  untitledTranscriptionLabel = 'Untitled transcription';

  constructor(
    private transcriptionService: TranscriptionService,
    private permissionService: PermissionService,
    private localizationService: LocalizationService,
    private modalService: NgbModal
  ) {
    this.actionLabel = this.t('Administration::Transcription:ActionLabel', 'Action');
    this.untitledTranscriptionLabel = this.t('Administration::Transcription:UntitledTranscription', 'Untitled transcription');
  }

  ngOnInit(): void {
    this.hasTranscriptionCreatePermission = this.permissionService.getGrantedPolicy('Workflows.Transcriptions.Create');
    this.hasTranscriptionViewPermission = this.permissionService.getGrantedPolicy('Workflows.Transcriptions.View');
    this.hasTranscriptionUpdatePermission = this.permissionService.getGrantedPolicy('Workflows.Transcriptions.Update');
    this.hasTranscriptionDeletePermission = this.permissionService.getGrantedPolicy('Workflows.Transcriptions.Delete');
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

  deleteTranscription(item: TranscriptionDto): void {
    if (!item?.id || this.deletingId) {
      return;
    }

    this.pendingDeleteItem = item;
    this.deleteConfirmationTitle = item.title?.trim() || this.untitledTranscriptionLabel;

    const modalRef = this.modalService.open(this.deleteTranscriptionModal, {
      centered: true,
      backdrop: 'static',
      size: 'md',
    });

    modalRef.result.then(
      () => this.confirmDeleteTranscription(),
      () => {
        this.pendingDeleteItem = null;
        this.deleteConfirmationTitle = '';
      }
    );
  }

  confirmDeleteTranscription(): void {
    if (!this.pendingDeleteItem?.id) {
      return;
    }

    const item = this.pendingDeleteItem;
    this.deletingId = item.id;
    this.error = null;

    this.transcriptionService.delete(item.id).subscribe({
      next: () => {
        this.transcriptions = this.transcriptions.filter(x => x.id !== item.id);
        this.deletingId = null;
        this.pendingDeleteItem = null;
        this.deleteConfirmationTitle = '';
      },
      error: () => {
        this.error = this.t('Administration::Transcription:DeleteFailedLabel', 'Unable to delete transcription.');
        this.deletingId = null;
        this.pendingDeleteItem = null;
        this.deleteConfirmationTitle = '';
      },
    });
  }

  private t(key: string, fallback: string): string {
    const value = this.localizationService.instant(key);
    return value && value !== key ? value : fallback;
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
