import type { EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { ServiceWorkflowHistoryType } from './service-workflow-history-type.enum';
import type { ServiceWorkflowInstanceStatus } from './service-workflow-instance-status.enum';
import type { ServiceWorkflowTaskStatus } from './service-workflow-task-status.enum';

export interface CreateUpdateServiceWorkflowDto {
  serviceId?: string;
  name?: string;
  description?: string;
  isActive?: boolean;
  steps: CreateUpdateServiceWorkflowStepDto[];
}

export interface CreateUpdateServiceWorkflowStepDto {
  serviceWorkflowId?: string;
  name?: string;
  description?: string;
  order?: number;
}

export interface CreateUpdateServiceWorkflowInstanceDto {
  serviceWorkflowId?: string;
  requestId?: string;
  currentStepId?: string;
  status?: ServiceWorkflowInstanceStatus;
}

export interface CreateUpdateServiceWorkflowTaskDto {
  serviceWorkflowInstanceId?: string;
  serviceWorkflowStepId?: string;
  title?: string;
  description?: string;
  assigneeUserId?: string;
  status?: ServiceWorkflowTaskStatus;
  dueDate?: string;
}

export interface CreateUpdateServiceWorkflowHistoryDto {
  serviceWorkflowInstanceId?: string;
  serviceWorkflowStepId?: string;
  serviceWorkflowTaskId?: string;
  type?: ServiceWorkflowHistoryType;
  action?: string;
  comment?: string;
  performedByUserId?: string;
}

export interface ServiceWorkflowStepDto extends EntityDto<string> {
  serviceWorkflowId?: string;
  name?: string;
  description?: string;
  order?: number;
}

export interface ServiceWorkflowDto extends FullAuditedEntityDto<string> {
  serviceId?: string;
  relationServiceId?: string;
  relationServiceWorkflowId?: string;
  name?: string;
  description?: string;
  isActive?: boolean;
  steps: ServiceWorkflowStepDto[];
}

export interface ServiceWorkflowInstanceDto extends FullAuditedEntityDto<string> {
  serviceWorkflowId?: string;
  requestId?: string;
  currentStepId?: string;
  status?: ServiceWorkflowInstanceStatus;
  startedAt?: string;
  completedAt?: string;
}

export interface ServiceWorkflowTaskDto extends FullAuditedEntityDto<string> {
  serviceWorkflowInstanceId?: string;
  serviceWorkflowStepId?: string;
  title?: string;
  description?: string;
  assigneeUserId?: string;
  status?: ServiceWorkflowTaskStatus;
  dueDate?: string;
  completedAt?: string;
}

export interface ServiceWorkflowHistoryDto extends EntityDto<string> {
  serviceWorkflowInstanceId?: string;
  serviceWorkflowStepId?: string;
  serviceWorkflowTaskId?: string;
  type?: ServiceWorkflowHistoryType;
  action?: string;
  comment?: string;
  performedByUserId?: string;
  performedAt?: string;
}
