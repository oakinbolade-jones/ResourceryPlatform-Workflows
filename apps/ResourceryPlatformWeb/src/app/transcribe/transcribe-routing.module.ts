import { eLayoutType } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EditTranscriptionsComponent } from './edit-transcriptions/edit-transcriptions.component';
import { ListTranscriptionComponent } from './list-transcription/list-transcription.component';
import { TranscribeComponent } from './transcribe.component';
import { ViewTranscriptionComponent } from './view-transcription/view-transcription.component';

const routes: Routes = [
  {
    path: '',
    component: TranscribeComponent,
    data: {
      layout: eLayoutType.application,
      title: 'Workflow::Transcribe',
    },
  },
  {
    path: 'list',
    component: ListTranscriptionComponent,
    data: {
      layout: eLayoutType.application,
      title: 'Workflow::TranscriptionList',
    },
  },
  {
    path: 'view/:id',
    component: ViewTranscriptionComponent,
    data: {
      layout: eLayoutType.application,
      title: 'Workflow::ViewTranscription',
    },
  },
  {
    path: 'edit/:id',
    component: EditTranscriptionsComponent,
    data: {
      layout: eLayoutType.application,
      title: 'Workflow::EditTranscription',
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TranscribeRoutingModule {}
