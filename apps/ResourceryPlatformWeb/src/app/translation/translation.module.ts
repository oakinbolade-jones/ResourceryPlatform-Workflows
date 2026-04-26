import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { TranslationRoutingModule } from './translation-routing.module';
import { TranslationComponent } from './translation.component';
import { TranslationLayoutComponent } from './translation-layout.component';
import { TranslationSidebarMenuComponent } from './translation-sidebar-menu.component';
import { TranslationRequestsComponent } from './translation-requests/translation-requests.component';
import { TranslationMountMeetingComponent } from './translation-mount-meeting/translation-mount-meeting.component';
import { TranslationProcessingComponent } from './translation-processing/translation-processing.component';
import { TranslationDocumentAlignmentComponent } from './translation-document-alignment/translation-document-alignment.component';
import { TranslatorPaymentComponent } from './translator-payment/translator-payment.component';
import { ApprovedRequestsComponent } from './approved-requests/approved-requests.component';
import { InProcessRequestsComponent } from './in-process-requests/in-process-requests.component';
import { RejectedRequestsComponent } from './rejected-requests/rejected-requests.component';
import { SlaComponent } from './sla/sla.component';
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';


@NgModule({
  declarations: [
    TranslationComponent,
    TranslationLayoutComponent,
    TranslationSidebarMenuComponent,
    TranslationRequestsComponent,
    TranslationMountMeetingComponent,
    TranslationProcessingComponent,
    TranslationDocumentAlignmentComponent,
    TranslatorPaymentComponent,
    ApprovedRequestsComponent,
    InProcessRequestsComponent,
    RejectedRequestsComponent,
    SlaComponent
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    TranslationRoutingModule,
    ResourceryLayoutModule
  ]
})
export class TranslationModule { }
