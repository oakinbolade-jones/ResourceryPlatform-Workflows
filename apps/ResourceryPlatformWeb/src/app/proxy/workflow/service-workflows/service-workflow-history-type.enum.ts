import { mapEnumToOptions } from '@abp/ng.core';

export enum ServiceWorkflowHistoryType {
  InstanceCreated = 0,
  StepStarted = 1,
  TaskCreated = 2,
  TaskCompleted = 3,
  TaskRejected = 4,
  InstanceCompleted = 5,
  InstanceCancelled = 6,
  CommentAdded = 7,
}

export const serviceWorkflowHistoryTypeOptions = mapEnumToOptions(ServiceWorkflowHistoryType);
