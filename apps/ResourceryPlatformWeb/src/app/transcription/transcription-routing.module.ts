import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TranscriptionComponent } from './transcription.component';
import { TranscriptionLayoutComponent } from './transcription-layout.component';
import { TranscriptionRequestsComponent } from './transcription-requests/transcription-requests.component';
import { TranscriptionProcessingComponent } from './transcription-processing/transcription-processing.component';
import { TranscriptionDocumentAlignmentComponent } from './transcription-document-alignment/transcription-document-alignment.component';
import { TranscriberPaymentComponent } from './transcriber-payment/transcriber-payment.component';

const routes: Routes = [
  {
    path: '',
    component: TranscriptionLayoutComponent,
    children: [
      { path: '', component: TranscriptionComponent, data: { title: 'Transcription' } },
      { path: 'transcription-requests', component: TranscriptionRequestsComponent, data: { title: 'Transcription Requests' } },
      { path: 'transcription-processing', component: TranscriptionProcessingComponent, data: { title: 'Transcription Processing' } },
      { path: 'transcription-document-alignment', component: TranscriptionDocumentAlignmentComponent, data: { title: 'Transcription Document Alignment' } },
      { path: 'transcriber-payment', component: TranscriberPaymentComponent, data: { title: 'Transcriber Payment' } }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TranscriptionRoutingModule { }
