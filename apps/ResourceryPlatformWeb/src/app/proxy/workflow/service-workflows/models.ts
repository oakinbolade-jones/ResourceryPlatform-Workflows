import type { ServiceWorkflowHistoryType } from './service-workflow-history-type.enum';
import type { ServiceWorkflowInstanceStatus } from './service-workflow-instance-status.enum';
import type { ServiceWorkflowTaskStatus } from './service-workflow-task-status.enum';
import type { EntityDto, FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateServiceWorkflowDto {
  name?: string;
  code?: string;
  displayName?: string;
  leadTime?: string;
  leadTimeType?: string;
  description?: string;
  isActive: boolean;
  steps: CreateUpdateServiceWorkflowStepDto[];
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

export interface CreateUpdateServiceWorkflowInstanceDto {
  serviceWorkflowId?: string;
  requestId?: string;
  currentStepId?: string;
  status?: ServiceWorkflowInstanceStatus;
}

export interface CreateUpdateServiceWorkflowStepDto {
  serviceWorkflowId?: string;
  name?: string;
  code?: string;
  description?: string;
  displayName?: string;
  displayNameOutput?: string;
  output?: string;
  tatType?: string;
  tatUnit?: string;
  order: number;
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

export interface ServiceWorkflowDto extends FullAuditedEntityDto<string> {
  name?: string;
  code?: string;
  displayName?: string;
  leadTime?: string;
  leadTimeType?: string;
  description?: string;
  isActive: boolean;
  steps: ServiceWorkflowStepDto[];
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

export interface ServiceWorkflowInstanceDto extends FullAuditedEntityDto<string> {
  serviceWorkflowId?: string;
  requestId?: string;
  currentStepId?: string;
  status?: ServiceWorkflowInstanceStatus;
  startedAt?: string;
  completedAt?: string;
}

export interface ServiceWorkflowStepDto extends EntityDto<string> {
  serviceWorkflowId?: string;
  name?: string;
  code?: string;
  description?: string;
  displayName?: string;
  displayNameOutput?: string;
  output?: string;
  tatType?: string;
  tatUnit?: string;
  order: number;
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
