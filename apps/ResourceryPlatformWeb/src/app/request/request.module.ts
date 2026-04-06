import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { RequestRoutingModule } from './request-routing.module';
import { RequestComponent } from './request.component';

@NgModule({
  declarations: [RequestComponent],
  imports: [SharedModule, RequestRoutingModule, ReactiveFormsModule],
})
export class RequestModule {}