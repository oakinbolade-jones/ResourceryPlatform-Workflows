import { Injectable } from '@angular/core';
import { Observable, switchMap } from 'rxjs';
import { CreateUpdateServiceDto, ServiceDto } from './models';
import { ServiceEntityService } from './service-entity.service';

@Injectable({ providedIn: 'root' })
export class ServiceClientProxyUsageExamples {
  constructor(private readonly serviceEntityService: ServiceEntityService) {}

  createServiceExample(name = 'Access Management Service'): Observable<ServiceDto> {
    const payload: CreateUpdateServiceDto = {
      name,
      description: 'Service endpoint ownership group for workflow definitions',
      isActive: true,
      serviceWorkflowIds: [],
    };

    return this.serviceEntityService.create(payload);
  }

  updateServiceExample(serviceId: string): Observable<ServiceDto> {
    return this.serviceEntityService.get(serviceId).pipe(
      switchMap(service =>
        this.serviceEntityService.update(serviceId, {
          name: service.name,
          description: `${service.description ?? ''} (updated)`.trim(),
          isActive: service.isActive,
          serviceWorkflowIds: (service.relations ?? [])
            .map(x => x.serviceWorkflowId)
            .filter((x): x is string => !!x),
        })
      )
    );
  }

  attachWorkflowExample(serviceId: string, serviceWorkflowId: string): Observable<ServiceDto> {
    return this.serviceEntityService.get(serviceId).pipe(
      switchMap(service => {
        const ids = new Set<string>(
          (service.relations ?? [])
            .map(x => x.serviceWorkflowId)
            .filter((x): x is string => !!x)
        );
        ids.add(serviceWorkflowId);

        return this.serviceEntityService.update(serviceId, {
          name: service.name,
          description: service.description,
          isActive: service.isActive,
          serviceWorkflowIds: Array.from(ids),
        });
      })
    );
  }

  deleteServiceExample(serviceId: string) {
    return this.serviceEntityService.delete(serviceId);
  }
}
