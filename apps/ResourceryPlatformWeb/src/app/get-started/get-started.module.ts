import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { GetStartedRoutingModule } from './get-started-routing.module'; 
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';
import { GetStartedComponent } from './get-started.component';
import { GetStartedLayoutComponent } from './get-started-layout.component';
import { GetStartedSidebarMenuComponent } from './get-started-sidebar-menu.component';


@NgModule({
  declarations: [
    GetStartedComponent,
    GetStartedLayoutComponent,
    GetStartedSidebarMenuComponent,   
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    GetStartedRoutingModule,
    ResourceryLayoutModule
  ]
})
export class GetStartedModule { }
