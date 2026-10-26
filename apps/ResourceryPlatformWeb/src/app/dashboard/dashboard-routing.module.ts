import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { DashboardLayoutComponent } from './dashboard-layout.component';
import { authGuard, AuthGuard, permissionGuard } from '@abp/ng.core';

const routes: Routes = [
  {
    path: '',
    component: DashboardLayoutComponent,
    children: [{
      path: '', component: DashboardComponent,
      canActivate: [authGuard, permissionGuard],
      data: {
        title: 'Workflow::Dashboard',

        requiredPolicy: "Workflows.ServiceCenters"
      }
    }]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
