import type { CreateUpdateServiceWorkflowTaskDto, ServiceWorkflowTaskDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceWorkflowTaskService {
  apiName = 'Workflow';
  

  create = (input: CreateUpdateServiceWorkflowTaskDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowTaskDto>({
      method: 'POST',
      url: '/api/workflow/service-workflow-tasks',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/workflow/service-workflow-tasks/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowTaskDto>({
      method: 'GET',
      url: `/api/workflow/service-workflow-tasks/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowTaskDto[]>({
      method: 'GET',
      url: '/api/workflow/service-workflow-tasks',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateServiceWorkflowTaskDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowTaskDto>({
      method: 'PUT',
      url: `/api/workflow/service-workflow-tasks/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
