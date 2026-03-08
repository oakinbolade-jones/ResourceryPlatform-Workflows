import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConferenceComponent } from './conference.component';
import { ConferenceLayoutComponent } from './conference-layout.component';
import { CalendarofEventsComponent } from './calendar-of-events/calendarof-events.component';
import { MeetingLogisticsCommissionComponent } from './meeting-logistics-commission/meeting-logistics-commission.component';
import { MeetingLogisticsExternalComponent } from './meeting-logistics-external/meeting-logistics-external.component';
import { MeetingLogisticsVirtualComponent } from './meeting-logistics-virtual/meeting-logistics-virtual.component';
import { MeetingLogisticsHybridComponent } from './meeting-logistics-hybrid/meeting-logistics-hybrid.component';
import { MeetingLogisticsCommunicationComponent } from './meeting-logistics-communication/meeting-logistics-communication.component';
import { InspectionIdentificationComponent } from './inspection-identification/inspection-identification.component';
import { ReproductionComponent } from './reproduction/reproduction.component';
import { PostEventComponent } from './post-event/post-event.component';
import { ConferenceRequestsComponent } from './conference-requests/conference-requests.component';
import { ApprovedRequestsComponent } from './approved-requests/approved-requests.component';
import { RejectedRequestsComponent } from './rejected-requests/rejected-requests.component';
import { InProcessRequestsComponent } from './in-process-requests/in-process-requests.component';
import { SlaComponent } from './sla/sla.component';

const routes: Routes = [
  {
    path: '',
    component: ConferenceLayoutComponent,
    children: [
      { path: '', component: ConferenceComponent },
      { path: 'calendar-of-events', component: CalendarofEventsComponent, data: { title: 'Calendar of Events' } },
      { path: 'meeting-logistics-commission', component: MeetingLogisticsCommissionComponent, data: { title: 'Meeting Logistics Commission' } },
      { path: 'meeting-logistics-external', component: MeetingLogisticsExternalComponent, data: { title: 'Meeting Logistics External' } },
      { path: 'meeting-logistics-virtual', component: MeetingLogisticsVirtualComponent, data: { title: 'Meeting Logistics Virtual' } },
      { path: 'meeting-logistics-hybrid', component: MeetingLogisticsHybridComponent, data: { title: 'Meeting Logistics Hybrid' } },
      { path: 'meeting-logistics-communication', component: MeetingLogisticsCommunicationComponent, data: { title: 'Meeting Logistics Communication' } },
      { path: 'inspection-identification', component: InspectionIdentificationComponent, data: { title: 'Inspection Identification' } },
      { path: 'reproduction', component: ReproductionComponent, data: { title: 'Reproduction' } },
      { path: 'post-event', component: PostEventComponent, data: { title: 'Post Event' } },
      { path: 'conference-requests', component: ConferenceRequestsComponent, data: { title: 'Conference Requests' } },
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
export class ConferenceRoutingModule { }
