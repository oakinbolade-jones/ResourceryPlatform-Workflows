import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface ServiceRelationDto {
  serviceId?: string;
  serviceWorkflowId?: string;
}

export interface ServiceDto extends FullAuditedEntityDto<string> {
  name?: string;
  description?: string;
  isActive?: boolean;
  relations: ServiceRelationDto[];
}

export interface CreateUpdateServiceDto {
  name?: string;
  description?: string;
  isActive?: boolean;
  serviceWorkflowIds: string[];
}
