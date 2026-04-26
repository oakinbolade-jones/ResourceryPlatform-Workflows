import type { CreateUpdateServiceWorkflowStepDto, ServiceWorkflowStepDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceWorkflowStepService {
  apiName = 'Workflow';
  

  create = (input: CreateUpdateServiceWorkflowStepDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowStepDto>({
      method: 'POST',
      url: '/api/workflow/service-workflow-steps',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/workflow/service-workflow-steps/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowStepDto>({
      method: 'GET',
      url: `/api/workflow/service-workflow-steps/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowStepDto[]>({
      method: 'GET',
      url: '/api/workflow/service-workflow-steps',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateServiceWorkflowStepDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowStepDto>({
      method: 'PUT',
      url: `/api/workflow/service-workflow-steps/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
