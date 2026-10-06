import { eLayoutType } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountComponent } from './account.component';
import { AccountRolesComponent } from './identity-roles/account-roles.component';
import { AccountUsersComponent } from './identity-users/account-users.component';
import { AccountTenantsComponent } from './tenant-management-tenants/account-tenants.component';
import { AccountSettingsComponent } from './setting-management-settings/account-settings.component';
import { AccountFeaturesComponent } from './feature-management-features/account-features.component';

const routes: Routes = [
  {
    path: '',
    component: AccountComponent,
    data: {
      layout: eLayoutType.application,
      title: 'Workflow::AccountAdministration',
    },
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'identity/roles' },
      { path: 'identity/roles', component: AccountRolesComponent },
      { path: 'identity/users', component: AccountUsersComponent },
      { path: 'tenant-management/tenants', component: AccountTenantsComponent },
      { path: 'setting-management/settings', component: AccountSettingsComponent },
      { path: 'feature-management/features', component: AccountFeaturesComponent },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountRoutingModule {}
