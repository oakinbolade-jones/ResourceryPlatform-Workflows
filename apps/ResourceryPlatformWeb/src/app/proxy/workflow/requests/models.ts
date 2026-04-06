import type { RequestType } from './request-type.enum';
import type { RequestStatus } from './request-status.enum';
import type { EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { DocumentMigrationStatus } from './document-migration-status.enum';

export interface CreateUpdateRequestDocumentDto {
  title?: string;
  description?: string;
  documentUrl?: string;
}

export interface CreateUpdateRequestDto {
  requestType?: RequestType;
  requestStatus?: RequestStatus;
  documentSetUrl?: string;
  description?: string;
  comment?: string;  
  serviceId?: string;
  documents: CreateUpdateRequestDocumentDto[];
}

export interface RequestDocumentDto extends EntityDto<string> {
  title?: string;
  description?: string;
  documentUrl?: string;
  sharePointDocumentUrl?: string;
  sharePointItemId?: string;
  migrationStatus?: DocumentMigrationStatus;
  lastMigrationError?: string;
  migratedAt?: string;
}

export interface RequestDto extends FullAuditedEntityDto<string> {
  requestType?: RequestType;
  requestStatus?: RequestStatus;
  documentSetUrl?: string;
  description?: string;
  comment?: string;
  serviceId?: string;
  documentMigrationStatus?: DocumentMigrationStatus;
  documentsPublishedAt?: string;
  documents: RequestDocumentDto[];
}
