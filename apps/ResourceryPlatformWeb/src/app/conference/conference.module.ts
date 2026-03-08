import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { ConferenceRoutingModule } from './conference-routing.module';
import { ConferenceComponent } from './conference.component';
import { ConferenceLayoutComponent } from './conference-layout.component';
import { ConferenceSidebarMenuComponent } from './conference-sidebar-menu.component';
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
import { InProcessRequestsComponent } from './in-process-requests/in-process-requests.component';
import { RejectedRequestsComponent } from './rejected-requests/rejected-requests.component';
import { SlaComponent } from './sla/sla.component';
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';


@NgModule({
  declarations: [
    ConferenceComponent,
    ConferenceLayoutComponent,
    ConferenceSidebarMenuComponent,
    CalendarofEventsComponent,
    MeetingLogisticsCommissionComponent,
    MeetingLogisticsExternalComponent,
    MeetingLogisticsVirtualComponent,
    MeetingLogisticsHybridComponent,
    MeetingLogisticsCommunicationComponent,
    InspectionIdentificationComponent,
    ReproductionComponent,
    PostEventComponent,
    ConferenceRequestsComponent,
    ApprovedRequestsComponent,
    InProcessRequestsComponent,
    RejectedRequestsComponent,
    SlaComponent
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    ConferenceRoutingModule,
    ResourceryLayoutModule
  ]
})
export class ConferenceModule { }
