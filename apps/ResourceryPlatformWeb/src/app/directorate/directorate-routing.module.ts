import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DirectorateComponent } from './directorate.component';
import { DirectorateLayoutComponent } from './directorate-layout.component';
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

const routes: Routes = [
  {
    path: '',
    component: DirectorateLayoutComponent,
    children: [
      { path: '', component: DirectorateComponent },
      { path: 'internal-memoranda', component: InternalMemorandaComponent, data: { title: 'Internal Memoranda' } },
      { path: 'incoming-correspondence', component: IncomingCorrespondenceComponent, data: { title: 'Incoming Correspondence' } },
      { path: 'note-verbales', component: NoteVerbalesComponent, data: { title: 'Note Verbales' } },
      { path: 'meeting-supervision', component: MeetingSupervisionComponent, data: { title: 'Meeting Supervision' } },
      { path: 'meeting-preparation', component: MeetingPreparationComponent, data: { title: 'Meeting Preparation' } },
      { path: 'contribution-reports', component: ContributionReportsComponent, data: { title: 'Contribution Reports' } },
      { path: 'concept-notes', component: ConceptNotesComponent, data: { title: 'Concept Notes' } },
      { path: 'internal-meetings', component: InternalMeetingsComponent, data: { title: 'Internal Meetings' } },
      { path: 'documentation', component: DocumentationComponent, data: { title: 'Documentation' } },
      { path: 'directorate-requests', component: DirectorateRequestsComponent, data: { title: 'Directorate Requests' } },
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
export class DirectorateRoutingModule { }