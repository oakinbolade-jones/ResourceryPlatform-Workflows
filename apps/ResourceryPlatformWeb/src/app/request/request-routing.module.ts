import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RequestComponent } from './request.component';
import { RequestLayoutComponent } from './request-layout.component';
import { RequestWorkflowProxyDemoComponent } from './request-workflow-proxy-demo.component';
import { AuthGuard } from '../shared/guards/auth.guard';

// To hide the header on any route, add hideHeader: true to the route data
// Example: data: { title: 'Requests', hideHeader: true }

const routes: Routes = [
  {
    path: '',
    component: RequestLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', component: RequestComponent, data: { title: 'Requests', hideHeader: true } },
      {
        path: 'proxy-demo',
        component: RequestWorkflowProxyDemoComponent,
        data: { title: 'Workflow Proxy Demo', hideHeader: true }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RequestRoutingModule {}
