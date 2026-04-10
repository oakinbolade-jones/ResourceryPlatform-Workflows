import type { CreateUpdateTranscriptionDto, TranscriptionDto } from './models';
import { Rest, RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TranscriptionService {
  apiName = 'Workflow';

  create = (input: CreateUpdateTranscriptionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TranscriptionDto>(
      {
        method: 'POST',
        url: '/api/workflow/transcription',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/workflow/transcription/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TranscriptionDto>(
      {
        method: 'GET',
        url: `/api/workflow/transcription/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  getBySourceReferenceId = (sourceReferenceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TranscriptionDto>(
      {
        method: 'GET',
        url: '/api/workflow/transcription/by-source-reference',
        params: { sourceReferenceId },
      },
      { apiName: this.apiName, ...config }
    );

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, TranscriptionDto[]>(
      {
        method: 'GET',
        url: '/api/workflow/transcription',
      },
      { apiName: this.apiName, ...config }
    );

  update = (id: string, input: CreateUpdateTranscriptionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TranscriptionDto>(
      {
        method: 'PUT',
        url: `/api/workflow/transcription/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  constructor(private restService: RestService) {}
}