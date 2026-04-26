import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LocalizationModule } from '@abp/ng.core';

import { ResourcerySidebarComponent } from './sidebar/sidebar.component';
import { ResourceryPageComponent } from './page/page.component';

@NgModule({
  declarations: [ResourcerySidebarComponent, ResourceryPageComponent],
  imports: [CommonModule, RouterModule, LocalizationModule],
  exports: [ResourcerySidebarComponent, ResourceryPageComponent]
})
export class ResourceryLayoutModule {}
