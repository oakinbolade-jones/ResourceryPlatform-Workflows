import type { CreateUpdateServiceDto, ServiceDto } from './models';
import { Rest, RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceEntityService {
  apiName = 'Workflow';

  create = (input: CreateUpdateServiceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceDto>(
      {
        method: 'POST',
        url: '/api/workflow/services',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/workflow/services/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceDto>(
      {
        method: 'GET',
        url: `/api/workflow/services/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceDto[]>(
      {
        method: 'GET',
        url: '/api/workflow/services',
      },
      { apiName: this.apiName, ...config }
    );

  update = (id: string, input: CreateUpdateServiceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceDto>(
      {
        method: 'PUT',
        url: `/api/workflow/services/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  constructor(private restService: RestService) {}
}
