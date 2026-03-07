import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ResourceryComponent } from './resourcery.component';

const routes: Routes = [
  {
    path: '',
    component: ResourceryComponent,
    children: [{ path: '', pathMatch: 'full', redirectTo: '/dashboard' }]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ResourceryRoutingModule { }
