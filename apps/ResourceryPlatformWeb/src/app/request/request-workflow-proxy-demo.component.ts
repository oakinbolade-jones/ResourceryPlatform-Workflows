import { Component } from '@angular/core';
import { ServiceWorkflowClientProxyUsageExamples } from '../proxy/workflow/service-workflows/usage-examples.service';
import { ServiceClientProxyUsageExamples } from '../proxy/workflow/services/usage-examples.service';

@Component({
  selector: 'app-request-workflow-proxy-demo',
  templateUrl: './request-workflow-proxy-demo.component.html',
  styleUrl: './request-workflow-proxy-demo.component.scss'
})
export class RequestWorkflowProxyDemoComponent {
  serviceId = '';
  serviceName = 'Demo Service';
  workflowId = '';
  requestId = '';
  stepId = '';
  instanceId = '';
  taskId = '';
  assigneeUserId = '';
  actorUserId = '';

  isBusy = false;
  lastMessage = 'Ready';
  lastPayload = '';

  constructor(
    private readonly examples: ServiceWorkflowClientProxyUsageExamples,
    private readonly serviceExamples: ServiceClientProxyUsageExamples
  ) {}

  createService(): void {
    this.start();
    this.serviceExamples.createServiceExample(this.serviceName || undefined).subscribe({
      next: service => {
        this.serviceId = service.id ?? '';
        this.success('Service created', service);
      },
      error: err => this.fail('Create service failed', err)
    });
  }

  updateService(): void {
    if (!this.serviceId) {
      this.lastMessage = 'Service ID is required';
      return;
    }

    this.start();
    this.serviceExamples.updateServiceExample(this.serviceId).subscribe({
      next: service => this.success('Service updated', service),
      error: err => this.fail('Update service failed', err)
    });
  }

  attachWorkflow(): void {
    if (!this.serviceId || !this.workflowId) {
      this.lastMessage = 'Service ID and Workflow ID are required';
      return;
    }

    this.start();
    this.serviceExamples.attachWorkflowExample(this.serviceId, this.workflowId).subscribe({
      next: service => this.success('Workflow attached to service', service),
      error: err => this.fail('Attach workflow failed', err)
    });
  }

  deleteService(): void {
    if (!this.serviceId) {
      this.lastMessage = 'Service ID is required';
      return;
    }

    this.start();
    this.serviceExamples.deleteServiceExample(this.serviceId).subscribe({
      next: () => this.success('Service deleted', { serviceId: this.serviceId }),
      error: err => this.fail('Delete service failed', err)
    });
  }

  createWorkflow(): void {
    if (!this.serviceId) {
      this.lastMessage = 'Service ID is required';
      return;
    }

    this.start();
    this.examples.createWorkflowExample(this.serviceId).subscribe({
      next: workflow => {
        this.workflowId = workflow.id ?? '';
        this.stepId = workflow.steps?.[0]?.id ?? '';
        this.success('Workflow created', workflow);
      },
      error: err => this.fail('Create workflow failed', err)
    });
  }

  updateWorkflow(): void {
    if (!this.workflowId) {
      this.lastMessage = 'Workflow ID is required';
      return;
    }

    this.start();
    this.examples.updateWorkflowExample(this.workflowId).subscribe({
      next: workflow => this.success('Workflow updated', workflow),
      error: err => this.fail('Update workflow failed', err)
    });
  }

  createStep(): void {
    if (!this.workflowId) {
      this.lastMessage = 'Workflow ID is required';
      return;
    }

    this.start();
    this.examples.createStepExample(this.workflowId).subscribe({
      next: step => {
        this.stepId = step.id ?? this.stepId;
        this.success('Step created', step);
      },
      error: err => this.fail('Create step failed', err)
    });
  }

  createInstance(): void {
    if (!this.workflowId || !this.requestId) {
      this.lastMessage = 'Workflow ID and Request ID are required';
      return;
    }

    this.start();
    this.examples.createInstanceExample(this.workflowId, this.requestId).subscribe({
      next: instance => {
        this.instanceId = instance.id ?? this.instanceId;
        this.success('Instance created', instance);
      },
      error: err => this.fail('Create instance failed', err)
    });
  }

  createTask(): void {
    if (!this.instanceId || !this.stepId || !this.assigneeUserId) {
      this.lastMessage = 'Instance ID, Step ID and Assignee User ID are required';
      return;
    }

    this.start();
    this.examples.createTaskExample(this.instanceId, this.stepId, this.assigneeUserId).subscribe({
      next: task => {
        this.taskId = task.id ?? this.taskId;
        this.success('Task created', task);
      },
      error: err => this.fail('Create task failed', err)
    });
  }

  completeTaskAndWriteHistory(): void {
    if (!this.taskId || !this.instanceId || !this.actorUserId) {
      this.lastMessage = 'Task ID, Instance ID and Actor User ID are required';
      return;
    }

    this.start();
    this.examples.completeTaskExample(this.taskId, this.instanceId, this.actorUserId).subscribe({
      next: history => this.success('Task completed and history created', history),
      error: err => this.fail('Complete task/history failed', err)
    });
  }

  deleteWorkflow(): void {
    if (!this.workflowId) {
      this.lastMessage = 'Workflow ID is required';
      return;
    }

    this.start();
    this.examples.deleteWorkflowExample(this.workflowId).subscribe({
      next: () => {
        this.success('Workflow deleted', { workflowId: this.workflowId });
      },
      error: err => this.fail('Delete workflow failed', err)
    });
  }

  private start(): void {
    this.isBusy = true;
    this.lastMessage = 'Running...';
  }

  private success(message: string, payload: unknown): void {
    this.isBusy = false;
    this.lastMessage = message;
    this.lastPayload = JSON.stringify(payload, null, 2);
  }

  private fail(message: string, error: unknown): void {
    this.isBusy = false;
    this.lastMessage = message;
    this.lastPayload = JSON.stringify(error, null, 2);
  }
}
