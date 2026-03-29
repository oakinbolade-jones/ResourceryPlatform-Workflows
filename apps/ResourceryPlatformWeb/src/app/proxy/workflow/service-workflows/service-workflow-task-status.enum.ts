import { mapEnumToOptions } from '@abp/ng.core';

export enum ServiceWorkflowTaskStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Rejected = 3,
  Cancelled = 4,
}

export const serviceWorkflowTaskStatusOptions = mapEnumToOptions(ServiceWorkflowTaskStatus);
