import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LocalizationModule } from '@abp/ng.core';

import { ResourcerySidebarComponent } from './sidebar/sidebar.component';
import { ResourceryPageComponent } from './page/page.component';
import { HidePageHeaderDirective } from '../../shared/directives/hide-page-header.directive';

@NgModule({
  declarations: [ResourcerySidebarComponent, ResourceryPageComponent, HidePageHeaderDirective],
  imports: [CommonModule, RouterModule, LocalizationModule],
  exports: [ResourcerySidebarComponent, ResourceryPageComponent, HidePageHeaderDirective]
})
export class ResourceryLayoutModule {}
