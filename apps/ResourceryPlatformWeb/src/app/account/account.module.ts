import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { PermissionManagementModule } from '@abp/ng.permission-management';
import { SharedModule } from '../shared/shared.module';
import { AccountRoutingModule } from './account-routing.module';
import { AccountComponent } from './account.component';
import { AccountRolesComponent } from './identity-roles/account-roles.component';
import { AccountUsersComponent } from './identity-users/account-users.component';
import { AccountTenantsComponent } from './tenant-management-tenants/account-tenants.component';
import { AccountSettingsComponent } from './setting-management-settings/account-settings.component';
import { AccountFeaturesComponent } from './feature-management-features/account-features.component';

@NgModule({
  declarations: [
    AccountComponent,
    AccountRolesComponent,
    AccountUsersComponent,
    AccountTenantsComponent,
    AccountSettingsComponent,
    AccountFeaturesComponent,
  ],
  imports: [SharedModule, AccountRoutingModule, ReactiveFormsModule, PermissionManagementModule],
})
export class AccountModule {}
