import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { InterpretationRoutingModule } from './interpretation-routing.module';
import { InterpretationComponent } from './interpretation.component';
import { InterpretationLayoutComponent } from './interpretation-layout.component';
import { InterpretationSidebarMenuComponent } from './interpretation-sidebar-menu.component';
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
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';


@NgModule({
  declarations: [
    InterpretationComponent,
    InterpretationLayoutComponent,
    InterpretationSidebarMenuComponent,
    InterpretationRequestsComponent,
    InterpretationProcessingComponent,
    InterpretaterDeploymentPhysicalComponent,
    InterpretaterDeploymentVirtualComponent,
    InterpretaterDeploymentHybridComponent,
    InterpretationInspectionComponent,
    InterpretationSoundCheckPhysicalComponent,
    InterpretationSoundCheckHybridComponent,
    InterpretationSoundCheckVirtualComponent,
    InterpreterPaymentComponent,
    InterpretationFeedbackComponent,
    ApprovedRequestsComponent,
    InProcessRequestsComponent,
    RejectedRequestsComponent,
    SlaComponent
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    InterpretationRoutingModule,
    ResourceryLayoutModule
  ]
})
export class InterpretationModule { }
