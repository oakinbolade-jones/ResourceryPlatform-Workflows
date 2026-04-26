import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { SupportRoutingModule } from './support-routing.module'; 
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';
import { SupportComponent } from './support.component';
import { SupportLayoutComponent } from './support-layout.component';
import { SupportSidebarMenuComponent } from './support-sidebar-menu.component';


@NgModule({
  declarations: [
    SupportComponent,
    SupportLayoutComponent,
    SupportSidebarMenuComponent,   
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    SupportRoutingModule,
    ResourceryLayoutModule
  ]
})
export class SupportModule { }
