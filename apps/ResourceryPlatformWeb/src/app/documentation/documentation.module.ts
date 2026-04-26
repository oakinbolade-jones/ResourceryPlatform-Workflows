import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { DocumentationRoutingModule } from './documentation-routing.module'; 
import { ResourceryLayoutModule } from '../resourcery/layout/resourcery-layout.module';
import { DocumentationComponent } from './documentation.component';
import { DocumentationLayoutComponent } from './documentation-layout.component';
import { DocumentationSidebarMenuComponent } from './documentation-sidebar-menu.component';


@NgModule({
  declarations: [
    DocumentationComponent,
    DocumentationLayoutComponent,
    DocumentationSidebarMenuComponent,   
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    DocumentationRoutingModule,
    ResourceryLayoutModule
  ]
})
export class DocumentationModule { }
