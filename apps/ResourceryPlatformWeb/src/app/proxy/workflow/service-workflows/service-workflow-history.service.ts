import type {
  CreateUpdateServiceWorkflowHistoryDto,
  ServiceWorkflowHistoryDto,
} from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceWorkflowHistoryService {
  apiName = 'Workflow';

  create = (input: CreateUpdateServiceWorkflowHistoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowHistoryDto>(
      {
        method: 'POST',
        url: '/api/workflow/service-workflow-history',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/workflow/service-workflow-history/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowHistoryDto>(
      {
        method: 'GET',
        url: `/api/workflow/service-workflow-history/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowHistoryDto[]>(
      {
        method: 'GET',
        url: '/api/workflow/service-workflow-history',
      },
      { apiName: this.apiName, ...config }
    );

  update = (
    id: string,
    input: CreateUpdateServiceWorkflowHistoryDto,
    config?: Partial<Rest.Config>
  ) =>
    this.restService.request<any, ServiceWorkflowHistoryDto>(
      {
        method: 'PUT',
        url: `/api/workflow/service-workflow-history/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  constructor(private restService: RestService) {}
}
