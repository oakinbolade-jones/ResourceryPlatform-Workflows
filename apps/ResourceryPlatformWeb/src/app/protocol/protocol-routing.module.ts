import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProtocolComponent } from './protocol.component';
import { ProtocolLayoutComponent } from './protocol-layout.component';
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

const routes: Routes = [
  {
    path: '',
    component: ProtocolLayoutComponent,
    children: [
      { path: '', component: ProtocolComponent, data: { title: 'Protocol' } },
      { path: 'protocol-ground-transportation', component: ProtocolGroundTransportationComponent, data: { title: 'Protocol Ground Transportation' } },
      { path: 'protocol-visa-procurement', component: ProtocolVisaProcurementComponent, data: { title: 'Protocol Visa Procurement' } },
      { path: 'protocol-accreditation-hom', component: ProtocolAccreditationHomComponent, data: { title: 'Protocol Accreditation Hom' } },
      { path: 'protocol-preparation-diplomatic-cocktail-reception', component: ProtocolPreparationDiplomaticCocktailReceptionComponent, data: { title: 'Protocol Diplomatic Cocktail Reception' } },
      { path: 'protocol-reception-staff', component: ProtocolReceptionStaffComponent, data: { title: 'Protocol Reception Staff' } },
      { path: 'protocol-custom-duty-waiver', component: ProtocolCustomDutyWaiverComponent, data: { title: 'Protocol Custom Duty Waiver' } },
      { path: 'protocol-election-observation', component: ProtocolElectionObservationComponent, data: { title: 'Protocol Election Observation' } },
      { path: 'protocol-requests', component: ProtocolRequestsComponent, data: { title: 'Protocol Requests' } },
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
export class ProtocolRoutingModule { }
