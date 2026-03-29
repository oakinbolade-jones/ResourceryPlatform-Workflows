import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LocalizationModule } from '@abp/ng.core';

import { RequestRoutingModule } from './request-routing.module';
import { RequestComponent } from './request.component';
import { RequestLayoutComponent } from './request-layout.component';
import { RequestSidebarMenuComponent } from './request-sidebar-menu.component';
import { RequestWorkflowProxyDemoComponent } from './request-workflow-proxy-demo.component';
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';

@NgModule({
  declarations: [
    RequestComponent,
    RequestLayoutComponent,
    RequestSidebarMenuComponent,
    RequestWorkflowProxyDemoComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    LocalizationModule,
    RequestRoutingModule,
    ResourceryLayoutModule
  ]
})
export class RequestModule {}
