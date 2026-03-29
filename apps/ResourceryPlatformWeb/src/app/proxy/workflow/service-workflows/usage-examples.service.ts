import { Injectable } from '@angular/core';
import { Observable, switchMap } from 'rxjs';
import {
  CreateUpdateServiceWorkflowDto,
  CreateUpdateServiceWorkflowHistoryDto,
  CreateUpdateServiceWorkflowInstanceDto,
  CreateUpdateServiceWorkflowTaskDto,
  ServiceWorkflowDto,
  ServiceWorkflowHistoryService,
  ServiceWorkflowHistoryType,
  ServiceWorkflowInstanceService,
  ServiceWorkflowInstanceStatus,
  ServiceWorkflowService,
  ServiceWorkflowStepService,
  ServiceWorkflowTaskService,
  ServiceWorkflowTaskStatus,
} from './index';

@Injectable({ providedIn: 'root' })
export class ServiceWorkflowClientProxyUsageExamples {
  constructor(
    private readonly serviceWorkflowService: ServiceWorkflowService,
    private readonly serviceWorkflowStepService: ServiceWorkflowStepService,
    private readonly serviceWorkflowInstanceService: ServiceWorkflowInstanceService,
    private readonly serviceWorkflowTaskService: ServiceWorkflowTaskService,
    private readonly serviceWorkflowHistoryService: ServiceWorkflowHistoryService
  ) {}

  createWorkflowExample(serviceId: string): Observable<ServiceWorkflowDto> {
    const payload: CreateUpdateServiceWorkflowDto = {
      serviceId,
      name: 'Access Request Approval',
      description: 'Approval workflow for service access request',
      isActive: true,
      steps: [
        {
          serviceWorkflowId: undefined,
          name: 'Line Manager Review',
          description: 'Manager validates business need',
          order: 1,
        },
        {
          serviceWorkflowId: undefined,
          name: 'Security Review',
          description: 'Security confirms role access',
          order: 2,
        },
      ],
    };

    return this.serviceWorkflowService.create(payload);
  }

  updateWorkflowExample(workflowId: string): Observable<ServiceWorkflowDto> {
    return this.serviceWorkflowService.get(workflowId).pipe(
      switchMap(workflow =>
        this.serviceWorkflowService.update(workflowId, {
          serviceId: workflow.serviceId,
          name: workflow.name,
          description: `${workflow.description ?? ''} (updated)`.trim(),
          isActive: workflow.isActive,
          steps: (workflow.steps ?? []).map(step => ({
            serviceWorkflowId: workflowId,
            name: step.name,
            description: step.description,
            order: step.order,
          })),
        })
      )
    );
  }

  createStepExample(serviceWorkflowId: string) {
    return this.serviceWorkflowStepService.create({
      serviceWorkflowId,
      name: 'Final Approval',
      description: 'Service owner final sign-off',
      order: 99,
    });
  }

  createInstanceExample(serviceWorkflowId: string, requestId: string) {
    const payload: CreateUpdateServiceWorkflowInstanceDto = {
      serviceWorkflowId,
      requestId,
      currentStepId: undefined,
      status: ServiceWorkflowInstanceStatus.Pending,
    };

    return this.serviceWorkflowInstanceService.create(payload);
  }

  createTaskExample(instanceId: string, stepId: string, assigneeUserId: string) {
    const payload: CreateUpdateServiceWorkflowTaskDto = {
      serviceWorkflowInstanceId: instanceId,
      serviceWorkflowStepId: stepId,
      title: 'Approve access request',
      description: 'Review and approve this request',
      assigneeUserId,
      status: ServiceWorkflowTaskStatus.Pending,
      dueDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000).toISOString(),
    };

    return this.serviceWorkflowTaskService.create(payload);
  }

  completeTaskExample(taskId: string, instanceId: string, userId: string) {
    return this.serviceWorkflowTaskService.get(taskId).pipe(
      switchMap(task =>
        this.serviceWorkflowTaskService.update(taskId, {
          serviceWorkflowInstanceId: task.serviceWorkflowInstanceId,
          serviceWorkflowStepId: task.serviceWorkflowStepId,
          title: task.title,
          description: task.description,
          assigneeUserId: task.assigneeUserId,
          dueDate: task.dueDate,
          status: ServiceWorkflowTaskStatus.Completed,
        })
      ),
      switchMap(() => {
        const historyPayload: CreateUpdateServiceWorkflowHistoryDto = {
          serviceWorkflowInstanceId: instanceId,
          type: ServiceWorkflowHistoryType.TaskCompleted,
          action: 'Task completed',
          comment: 'Completed from Angular client proxy usage example',
          performedByUserId: userId,
        };

        return this.serviceWorkflowHistoryService.create(historyPayload);
      })
    );
  }

  deleteWorkflowExample(workflowId: string) {
    return this.serviceWorkflowService.delete(workflowId);
  }
}
