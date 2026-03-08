import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { ProtocolRoutingModule } from './protocol-routing.module';
import { ProtocolComponent } from './protocol.component';
import { ProtocolLayoutComponent } from './protocol-layout.component';
import { ProtocolSidebarMenuComponent } from './protocol-sidebar-menu.component';
import { ProtocolGroundTransportationComponent } from './protocol-ground-transportation/protocol-ground-transportation.component';
import { ProtocolVisaProcurementComponent } from './protocol-visa-procurement/protocol-visa-procurement.component';
import { ProtocolAccreditationHomComponent } from './protocol-accreditation-hom/protocol-accreditation-hom.component';
import { ProtocolPreparationDiplomaticCocktailReceptionComponent } from './protocol-preparation-diplomatic-cocktail-reception/protocol-preparation-diplomatic-cocktail-reception.component';
import { ProtocolReceptionStaffComponent } from './protocol-reception-staff/protocol-reception-staff.component';
import { ProtocolCustomDutyWaiverComponent } from './protocol-custom-duty-waiver/protocol-custom-duty-waiver.component';
import { ProtocolElectionObservationComponent } from './protocol-election-observation/protocol-election-observation.component';
import { ProtocolRequestsComponent } from './protocol-requests/protocol-requests.component';
import { ApprovedRequestsComponent } from './approved-requests/approved-requests.component';
import { InProcessRequestsComponent } from './in-process-requests/in-process-requests.component';
import { RejectedRequestsComponent } from './rejected-requests/rejected-requests.component';
import { SlaComponent } from './sla/sla.component';
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';


@NgModule({
  declarations: [
    ProtocolComponent,
    ProtocolLayoutComponent,
    ProtocolSidebarMenuComponent,
    ProtocolGroundTransportationComponent,
    ProtocolVisaProcurementComponent,
    ProtocolAccreditationHomComponent,
    ProtocolPreparationDiplomaticCocktailReceptionComponent,
    ProtocolReceptionStaffComponent,
    ProtocolCustomDutyWaiverComponent,
    ProtocolElectionObservationComponent,
    ProtocolRequestsComponent,
    ApprovedRequestsComponent,
    InProcessRequestsComponent,
    RejectedRequestsComponent,
    SlaComponent
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    ProtocolRoutingModule,
    ResourceryLayoutModule
  ]
})
export class ProtocolModule { }
