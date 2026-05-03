import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DocumentationComponent } from './documentation.component';
import { DocumentationLayoutComponent } from './documentation-layout.component';
 

const routes: Routes = [
  {
    path: '',
    component: DocumentationLayoutComponent,
    children: [
      { path: '', component: DocumentationComponent },      
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DocumentationRoutingModule { }
