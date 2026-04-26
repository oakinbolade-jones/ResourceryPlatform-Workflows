import type { CreateUpdateServiceCenterDto, ServiceCenterDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceCenterService {
  apiName = 'Workflow';
  

  create = (input: CreateUpdateServiceCenterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceCenterDto>({
      method: 'POST',
      url: '/api/workflow/service-centers',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/workflow/service-centers/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceCenterDto>({
      method: 'GET',
      url: `/api/workflow/service-centers/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceCenterDto[]>({
      method: 'GET',
      url: '/api/workflow/service-centers',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateServiceCenterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceCenterDto>({
      method: 'PUT',
      url: `/api/workflow/service-centers/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
