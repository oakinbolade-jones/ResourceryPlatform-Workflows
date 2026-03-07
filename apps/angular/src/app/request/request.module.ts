import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LocalizationModule } from '@abp/ng.core';

import { RequestRoutingModule } from './request-routing.module';
import { RequestComponent } from './request.component';
import { RequestLayoutComponent } from './request-layout.component';
import { RequestSidebarMenuComponent } from './request-sidebar-menu.component';
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';

@NgModule({
  declarations: [
    RequestComponent,
    RequestLayoutComponent,
    RequestSidebarMenuComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    LocalizationModule,
    RequestRoutingModule,
    ResourceryLayoutModule
  ]
})
export class RequestModule {}
