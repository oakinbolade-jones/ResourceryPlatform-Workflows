import { mapEnumToOptions } from '@abp/ng.core';

export enum ServiceWorkflowInstanceStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Rejected = 3,
  Cancelled = 4,
}

export const serviceWorkflowInstanceStatusOptions = mapEnumToOptions(ServiceWorkflowInstanceStatus);
