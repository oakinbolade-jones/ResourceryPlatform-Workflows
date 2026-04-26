import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { TranscribeRoutingModule } from './transcribe-routing.module';
import { TranscribeComponent } from './transcribe.component';
import { ListTranscriptionComponent } from './list-transcription/list-transcription.component';
import { ViewTranscriptionComponent } from './view-transcription/view-transcription.component';
import { EditTranscriptionsComponent } from './edit-transcriptions/edit-transcriptions.component';

@NgModule({
  declarations: [
    TranscribeComponent,
    ViewTranscriptionComponent,
    ListTranscriptionComponent,
    EditTranscriptionsComponent,
  ],
  imports: [SharedModule, TranscribeRoutingModule, ReactiveFormsModule],
})
export class TranscribeModule {}
