import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InterpretationComponent } from './interpretation.component';
import { InterpretationLayoutComponent } from './interpretation-layout.component';
import { InterpretationRequestsComponent } from './interpretation-requests/interpretation-requests.component';
import { InterpretationProcessingComponent } from './interpretation-processing/interpretation-processing.component';
import { InterpretaterDeploymentPhysicalComponent } from './interpretater-deployment-physical/interpretater-deployment-physical.component';
import { InterpretaterDeploymentVirtualComponent } from './interpretater-deployment-virtual/interpretater-deployment-virtual.component';
import { InterpretaterDeploymentHybridComponent } from './interpretater-deployment-hybrid/interpretater-deployment-hybrid.component';
import { InterpretationInspectionComponent } from './interpretation-inspection/interpretation-inspection.component';
import { InterpretationSoundCheckPhysicalComponent } from './interpretation-sound-check-physical/interpretation-sound-check-physical.component';
import { InterpretationSoundCheckHybridComponent } from './interpretation-sound-check-hybrid/interpretation-sound-check-hybrid.component';
import { InterpretationSoundCheckVirtualComponent } from './interpretation-sound-check-virtual/interpretation-sound-check-virtual.component';
import { InterpreterPaymentComponent } from './interpreter-payment/interpreter-payment.component';
import { InterpretationFeedbackComponent } from './interpretation-feedback/interpretation-feedback.component';
import { ApprovedRequestsComponent } from './approved-requests/approved-requests.component';
import { InProcessRequestsComponent } from './in-process-requests/in-process-requests.component';
import { RejectedRequestsComponent } from './rejected-requests/rejected-requests.component';
import { SlaComponent } from './sla/sla.component';

const routes: Routes = [
  {
    path: '',
    component: InterpretationLayoutComponent,
    children: [
      { path: '', component: InterpretationComponent, data: { title: 'Interpretation' } },
      { path: 'interpretation-requests', component: InterpretationRequestsComponent, data: { title: 'Interpretation Requests' } },
      { path: 'interpretation-processing', component: InterpretationProcessingComponent, data: { title: 'Interpretation Processing' } },
      { path: 'interpretater-deployment-physical', component: InterpretaterDeploymentPhysicalComponent, data: { title: 'Interpretater Deployment Physical' } },
      { path: 'interpretater-deployment-virtual', component: InterpretaterDeploymentVirtualComponent, data: { title: 'Interpretater Deployment Virtual' } },
      { path: 'interpretater-deployment-hybrid', component: InterpretaterDeploymentHybridComponent, data: { title: 'Interpretater Deployment Hybrid' } },
      { path: 'interpretation-inspection', component: InterpretationInspectionComponent, data: { title: 'Interpretation Inspection' } },
      { path: 'interpretation-sound-check-physical', component: InterpretationSoundCheckPhysicalComponent, data: { title: 'Interpretation Sound Check Physical' } },
      { path: 'interpretation-sound-check-hybrid', component: InterpretationSoundCheckHybridComponent, data: { title: 'Interpretation Sound Check Hybrid' } },
      { path: 'interpretation-sound-check-virtual', component: InterpretationSoundCheckVirtualComponent, data: { title: 'Interpretation Sound Check Virtual' } },
      { path: 'interpreter-payment', component: InterpreterPaymentComponent, data: { title: 'Interpreter Payment' } },
      { path: 'interpretation-feedback', component: InterpretationFeedbackComponent, data: { title: 'Interpretation Feedback' } },
      { path: 'approved-requests', component: ApprovedRequestsComponent, data: { title: 'Approved Requests' } },
      { path: 'in-process-requests', component: InProcessRequestsComponent, data: { title: 'In Process Requests' } },
      { path: 'rejected-requests', component: RejectedRequestsComponent, data: { title: 'Rejected Requests' } },
      { path: 'sla', component: SlaComponent, data: { title: 'SLA' } }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InterpretationRoutingModule { }
