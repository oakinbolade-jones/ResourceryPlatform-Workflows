import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { WebcastRoutingModule } from './webcast-routing.module'; 
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';
import { WebcastComponent } from './webcast.component';
import { WebcastLayoutComponent } from './webcast-layout.component';
import { WebcastSidebarMenuComponent } from './webcast-sidebar-menu.component';


@NgModule({
  declarations: [
    WebcastComponent,
    WebcastLayoutComponent,
    WebcastSidebarMenuComponent,   
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    WebcastRoutingModule,
    ResourceryLayoutModule
  ]
})
export class WebcastModule { }
