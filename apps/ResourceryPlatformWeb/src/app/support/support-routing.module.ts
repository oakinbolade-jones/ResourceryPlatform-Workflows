import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupportComponent } from './support.component';
import { SupportLayoutComponent } from './support-layout.component';
 

const routes: Routes = [
  {
    path: '',
    component: SupportLayoutComponent,
    children: [
      { path: '', component: SupportComponent },      
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupportRoutingModule { }
