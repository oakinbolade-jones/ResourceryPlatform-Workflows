import { eLayoutType } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RequestComponent } from './request.component';

const routes: Routes = [
  {
    path: '',
    component: RequestComponent,
    data: {
      layout: eLayoutType.application,
      title: 'Workflow::Requests'
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RequestRoutingModule {}