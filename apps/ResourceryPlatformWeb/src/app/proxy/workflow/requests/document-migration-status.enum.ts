import { mapEnumToOptions } from '@abp/ng.core';

export enum DocumentMigrationStatus {
  NotStarted = 0,
  Pending = 1,
  InProgress = 2,
  Completed = 3,
  Failed = 4,
}

export const documentMigrationStatusOptions = mapEnumToOptions(DocumentMigrationStatus);
