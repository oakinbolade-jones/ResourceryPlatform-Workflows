import type { CreateUpdateRequestDocumentDto, CreateUpdateRequestDto, RequestDto } from './models';
import type { RequestStatus } from './request-status.enum';
import type { RequestType } from './request-type.enum';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RequestService {
  apiName = 'Workflow';

  addDocuments = (id: string, documents: CreateUpdateRequestDocumentDto[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, RequestDto>({
      method: 'POST',
      url: `/api/workflow/requests/${id}/documents`,
      body: documents,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RequestDto>({
      method: 'POST',
      url: '/api/workflow/requests',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/workflow/requests/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RequestDto>({
      method: 'GET',
      url: `/api/workflow/requests/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByStatus = (requestStatus: RequestStatus, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RequestDto[]>({
      method: 'GET',
      url: `/api/workflow/requests/status/${requestStatus}`,
    },
    { apiName: this.apiName,...config });
  

  getByType = (requestType: RequestType, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RequestDto[]>({
      method: 'GET',
      url: `/api/workflow/requests/type/${requestType}`,
    },
    { apiName: this.apiName,...config });
  

  getByUser = (userId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RequestDto[]>({
      method: 'GET',
      url: `/api/workflow/requests/user/${userId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, RequestDto[]>({
      method: 'GET',
      url: '/api/workflow/requests',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RequestDto>({
      method: 'PUT',
      url: `/api/workflow/requests/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
