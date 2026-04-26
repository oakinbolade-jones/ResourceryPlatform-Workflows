import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

import { ResourceryRoutingModule } from './resourcery-routing.module';
import { ResourceryComponent } from './resourcery.component';
import { ResourceryLayoutModule } from './layout/resourcery-layout.module';
import { LogoComponent } from './layout/logo/logo.component';


@NgModule({
  declarations: [
    ResourceryComponent,
  ],
  imports: [
    CommonModule,
    LocalizationModule,
    ResourceryRoutingModule,
    ResourceryLayoutModule,
    LogoComponent
  ]
})
export class ResourceryModule { }
