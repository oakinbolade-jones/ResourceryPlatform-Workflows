import { Injectable } from '@angular/core';
import { Observable, switchMap } from 'rxjs';
import { CreateUpdateServiceDto, ServiceDto } from './models';
import { WorkflowService } from './workflow.service';

@Injectable({ providedIn: 'root' })
export class ServiceClientProxyUsageExamples {
  constructor(private readonly workflowService: WorkflowService) {}

  createServiceExample(name = 'Access Management Service'): Observable<ServiceDto> {
    const payload: CreateUpdateServiceDto = {
      name,
      code: 'ACCESS-MGMT',
      displayName: name,
      serviceCenterId: undefined,
      description: 'Service endpoint ownership group for workflow definitions',
      isActive: true,
    };

    return this.workflowService.create(payload);
  }

  updateServiceExample(serviceId: string): Observable<ServiceDto> {
    return this.workflowService.get(serviceId).pipe(
      switchMap((service: ServiceDto) =>
        this.workflowService.update(serviceId, {
          serviceCenterId: service.serviceCenterId,
          name: service.name,
          code: service.code,
          displayName: service.displayName,
          description: `${service.description ?? ''} (updated)`.trim(),
          isActive: service.isActive,
        })
      )
    );
  }

  setServiceCenterExample(serviceId: string, serviceCenterId: string): Observable<ServiceDto> {
    return this.workflowService.get(serviceId).pipe(
      switchMap((service: ServiceDto) =>
        this.workflowService.update(serviceId, {
          serviceCenterId,
          name: service.name,
          code: service.code,
          displayName: service.displayName,
          description: service.description,
          isActive: service.isActive,
        })
      )
    );
  }

  deleteServiceExample(serviceId: string) {
    return this.workflowService.delete(serviceId);
  }
}
