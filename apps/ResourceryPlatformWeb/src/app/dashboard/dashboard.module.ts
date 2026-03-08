import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { DashboardLayoutComponent } from './dashboard-layout.component';
import { DashboardSidebarMenuComponent } from './dashboard-sidebar-menu.component';
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';

@NgModule({
  declarations: [
    DashboardComponent,
    DashboardLayoutComponent,
    DashboardSidebarMenuComponent
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    DashboardRoutingModule,
    ResourceryLayoutModule
  ]
})
export class DashboardModule {}
