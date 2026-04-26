import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LocalizationModule } from '@abp/ng.core';

import { DirectorateRoutingModule } from './directorate-routing.module';
import { DirectorateComponent } from './directorate.component';
import { DirectorateLayoutComponent } from './directorate-layout.component';
import { DirectorateSidebarMenuComponent } from './directorate-sidebar-menu.component';
import { InternalMemorandaComponent } from './internal-memoranda/internal-memoranda.component';
import { IncomingCorrespondenceComponent } from './incoming-correspondence/incoming-correspondence.component';
import { NoteVerbalesComponent } from './note-verbales/note-verbales.component';
import { MeetingSupervisionComponent } from './meeting-supervision/meeting-supervision.component';
import { MeetingPreparationComponent } from './meeting-preparation/meeting-preparation.component';
import { ContributionReportsComponent } from './contribution-reports/contribution-reports.component';
import { ConceptNotesComponent } from './concept-notes/concept-notes.component';
import { InternalMeetingsComponent } from './internal-meetings/internal-meetings.component';
import { DocumentationComponent } from './documentation/documentation.component';
import { DirectorateRequestsComponent } from './directorate-requests/directorate-requests.component';
import { ApprovedRequestsComponent } from './approved-requests/approved-requests.component';
import { InProcessRequestsComponent } from './in-process-requests/in-process-requests.component';
import { RejectedRequestsComponent } from './rejected-requests/rejected-requests.component';
import { SlaComponent } from './sla/sla.component';
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';


@NgModule({
  declarations: [
    DirectorateComponent,
    DirectorateLayoutComponent,
    DirectorateSidebarMenuComponent,
    InternalMemorandaComponent,
    IncomingCorrespondenceComponent,
    NoteVerbalesComponent,
    MeetingSupervisionComponent,
    MeetingPreparationComponent,
    ContributionReportsComponent,
    ConceptNotesComponent,
    InternalMeetingsComponent,
    DocumentationComponent,
    DirectorateRequestsComponent,
    ApprovedRequestsComponent,
    InProcessRequestsComponent,
    RejectedRequestsComponent,
    SlaComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    DirectorateRoutingModule,
    LocalizationModule,
    ResourceryLayoutModule
  ]
})
export class DirectorateModule { }
