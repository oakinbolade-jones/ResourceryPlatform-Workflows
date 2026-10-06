import { eLayoutType } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes, RouterModule } from '@angular/router';
import { EditTranscriptionsComponent } from './edit-transcriptions/edit-transcriptions.component';
import { ListTranscriptionComponent } from './list-transcription/list-transcription.component';
import { TranscribeComponent } from './transcribe.component';
import { ViewTranscriptionComponent } from './view-transcription/view-transcription.component';

const routes: Routes = [
  {
    path: '',
    component: TranscribeComponent,
    canActivate: [authGuard, permissionGuard],
    data: {
      layout: eLayoutType.application,
      title: 'Administration::Transcribe',
      requiredPolicy: 'Workflows.Transcriptions.Create'
    },
  },
  {
    path: 'list',
    component: ListTranscriptionComponent,
    canActivate: [authGuard, permissionGuard],
    data: {
      layout: eLayoutType.application,
      title: 'Administration::TranscriptionList',
        requiredPolicy: 'Workflows.Transcriptions.List'
    },
  },
  {
    path: 'view/:id',
    component: ViewTranscriptionComponent,
    canActivate: [authGuard, permissionGuard],
    data: {
      layout: eLayoutType.application,
      title: 'Administration::ViewTranscription',
      requiredPolicy: 'Workflows.Transcriptions.View'
    },
  },
  {
    path: 'edit/:id',
    component: EditTranscriptionsComponent,
    canActivate: [authGuard, permissionGuard],
    data: {
      layout: eLayoutType.application,
      title: 'Administration::EditTranscription',
      requiredPolicy: 'Workflows.Transcriptions.Update'
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TranscribeRoutingModule { }
