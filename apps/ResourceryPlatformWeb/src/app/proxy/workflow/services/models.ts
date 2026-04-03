import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateServiceCenterDto {
  name?: string;
  displayName?: string;
  description?: string;
  code?: string;
}

export interface CreateUpdateServiceDto {
  serviceCenterId?: string;
  name?: string;
  code?: string;
  displayName?: string;
  description?: string;
  isActive: boolean;
}

export interface ServiceCenterDto extends FullAuditedEntityDto<string> {
  name?: string;
  displayName?: string;
  description?: string;
  code?: string;
}

export interface ServiceDto extends FullAuditedEntityDto<string> {
  serviceCenterId?: string;
  name?: string;
  code?: string;
  displayName?: string;
  description?: string;
  isActive: boolean;
}
