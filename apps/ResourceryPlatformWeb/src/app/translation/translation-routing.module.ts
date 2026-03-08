import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TranslationComponent } from './translation.component';
import { TranslationLayoutComponent } from './translation-layout.component';
import { TranslationRequestsComponent } from './translation-requests/translation-requests.component';
import { TranslationMountMeetingComponent } from './translation-mount-meeting/translation-mount-meeting.component';
import { TranslationProcessingComponent } from './translation-processing/translation-processing.component';
import { TranslationDocumentAlignmentComponent } from './translation-document-alignment/translation-document-alignment.component';
import { TranslatorPaymentComponent } from './translator-payment/translator-payment.component';
import { ApprovedRequestsComponent } from './approved-requests/approved-requests.component';
import { InProcessRequestsComponent } from './in-process-requests/in-process-requests.component';
import { RejectedRequestsComponent } from './rejected-requests/rejected-requests.component';
import { SlaComponent } from './sla/sla.component';

const routes: Routes = [
  {
    path: '',
    component: TranslationLayoutComponent,
    children: [
      { path: '', component: TranslationComponent },
      { path: 'translation-requests', component: TranslationRequestsComponent, data: { title: 'Translation Requests' } },
      { path: 'translation-mount-meeting', component: TranslationMountMeetingComponent, data: { title: 'Translation Mount Meeting' } },
      { path: 'translation-processing', component: TranslationProcessingComponent, data: { title: 'Translation Processing' } },
      { path: 'translation-document-alignment', component: TranslationDocumentAlignmentComponent, data: { title: 'Translation Document Alignment' } },
      { path: 'translator-payment', component: TranslatorPaymentComponent, data: { title: 'Translator Payment' } },
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
export class TranslationRoutingModule { }
