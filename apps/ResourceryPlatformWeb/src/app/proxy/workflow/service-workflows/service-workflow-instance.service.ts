import type { CreateUpdateServiceWorkflowInstanceDto, ServiceWorkflowInstanceDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceWorkflowInstanceService {
  apiName = 'Workflow';
  

  create = (input: CreateUpdateServiceWorkflowInstanceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowInstanceDto>({
      method: 'POST',
      url: '/api/workflow/service-workflow-instances',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/workflow/service-workflow-instances/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowInstanceDto>({
      method: 'GET',
      url: `/api/workflow/service-workflow-instances/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowInstanceDto[]>({
      method: 'GET',
      url: '/api/workflow/service-workflow-instances',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateServiceWorkflowInstanceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceWorkflowInstanceDto>({
      method: 'PUT',
      url: `/api/workflow/service-workflow-instances/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
