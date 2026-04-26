import type { RequestType } from './request-type.enum';
import type { RequestStatus } from './request-status.enum';
import type { EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { DocumentMigrationStatus } from './document-migration-status.enum';

export interface CreateUpdateMeetingDto {
  title?: string;
  departureDate?: string;
  startDate?: string;
  endDate?: string;
  type?: number;
  referenceNumber?: string;
  numberOfParticipants?: number;
  location?: string;
  contactPhone?: string;
  contactEmail?: string;
  contactName?: string;
  hostName?: string;
  hostDesignation?: string;
  hostPhoneNumber?: string;
  hostEmail?: string;
  coHost1Name?: string;
  coHost1Designation?: string;
  coHost1PhoneNumber?: string;
  coHost1Email?: string;
  coHost2Name?: string;
  coHost2Designation?: string;
  coHost2PhoneNumber?: string;
  coHost2Email?: string;
  gLNumberRefreshments?: string;
  gLNumberHotel?: string;
  gLNumberCarHire?: string;
  gLNumberEquipment?: string;
  gLNumberLanguageServices?: string;
  costCenterNumberRefreshments?: string;
  costCenterNumberHotel?: string;
  costCenterNumberCarHire?: string;
  costCenterNumberEquipment?: string;
  costCenterNumberLanguageServices?: string;
}

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
  meetingForm?: CreateUpdateMeetingDto;
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

export interface MeetingDto extends EntityDto<string> {
  title?: string;
  departureDate?: string;
  startDate?: string;
  endDate?: string;
  type?: number;
  referenceNumber?: string;
  numberOfParticipants?: number;
  location?: string;
  contactPhone?: string;
  contactEmail?: string;
  contactName?: string;
  hostName?: string;
  hostDesignation?: string;
  hostPhoneNumber?: string;
  hostEmail?: string;
  coHost1Name?: string;
  coHost1Designation?: string;
  coHost1PhoneNumber?: string;
  coHost1Email?: string;
  coHost2Name?: string;
  coHost2Designation?: string;
  coHost2PhoneNumber?: string;
  coHost2Email?: string;
  gLNumberRefreshments?: string;
  gLNumberHotel?: string;
  gLNumberCarHire?: string;
  gLNumberEquipment?: string;
  gLNumberLanguageServices?: string;
  costCenterNumberRefreshments?: string;
  costCenterNumberHotel?: string;
  costCenterNumberCarHire?: string;
  costCenterNumberEquipment?: string;
  costCenterNumberLanguageServices?: string;
}

export interface RequestDto extends FullAuditedEntityDto<string> {
  requestType?: RequestType;
  requestStatus?: RequestStatus;
  documentSetUrl?: string;
  description?: string;
  comment?: string;
  serviceId?: string;
  meetingForm?: MeetingDto;
  documentMigrationStatus?: DocumentMigrationStatus;
  documentsPublishedAt?: string;
  documents: RequestDocumentDto[];
}
