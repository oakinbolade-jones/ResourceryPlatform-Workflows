import { eLayoutType } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountManageRedirectComponent } from './account/account-manage-redirect.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
    data: {
      layout: eLayoutType.empty,
      title: 'Workflow::Home'
    },
  },
  { path: 'request', loadChildren: () => import('./request/request.module').then(m => m.RequestModule) },
  { path: 'get-started', loadChildren: () => import('./get-started/get-started.module').then(m => m.GetStartedModule) },
  { path: 'documentation', loadChildren: () => import('./documentation/documentation.module').then(m => m.DocumentationModule) },
<<<<<<< HEAD
  { path: 'webcast', loadChildren: () => import('./webcast/webcast.module').then(m => m.WebcastModule), data: { title: 'Workflow::Webcasts' } },
  { path: 'support', loadChildren: () => import('./support/support.module').then(m => m.SupportModule), data: { title: 'Workflow::Support' } },
=======
  { path: 'webcast', loadChildren: () => import('./webcast/webcast.module').then(m => m.WebcastModule) },
  { path: 'support', loadChildren: () => import('./support/support.module').then(m => m.SupportModule) },
  {
    path: 'account-module',
    loadChildren: () => import('./account/account.module').then(m => m.AccountModule),
  },
>>>>>>> staging

  {
    path: 'account/manage',
    component: AccountManageRedirectComponent,
    data: { layout: eLayoutType.empty },
  },

  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(m => m.AccountModule.forLazy()), data: { title: 'Workflow::AccountManagement' }
  },
  {
    path: 'identity',
<<<<<<< HEAD
    loadChildren: () => import('@abp/ng.identity').then(m => m.IdentityModule.forLazy()), data: { title: 'Workflow::IdentityManagement' }
  },
  {
    path: 'tenant-management',
    loadChildren: () =>
      import('@abp/ng.tenant-management').then(m => m.TenantManagementModule.forLazy()), data: { title: 'Workflow::TenantManagament' }
  },
  {
    path: 'setting-management',
    loadChildren: () =>
      import('@abp/ng.setting-management').then(m => m.SettingManagementModule.forLazy()), data: { title: 'Workflow::SettingsManagement' }
=======
    pathMatch: 'full',
    redirectTo: 'account-module/identity/roles',
  },
  {
    path: 'identity/roles',
    pathMatch: 'full',
    redirectTo: 'account-module/identity/roles',
  },
  {
    path: 'identity/users',
    pathMatch: 'full',
    redirectTo: 'account-module/identity/users',
  },
  {
    path: 'tenant-management',
    pathMatch: 'full',
    redirectTo: 'account-module/tenant-management/tenants',
  },
  {
    path: 'tenant-management/tenants',
    pathMatch: 'full',
    redirectTo: 'account-module/tenant-management/tenants',
  },
  {
    path: 'setting-management',
    pathMatch: 'full',
    redirectTo: 'account-module/setting-management/settings',
  },
  {
    path: 'feature-management',
    pathMatch: 'full',
    redirectTo: 'account-module/feature-management/features',
>>>>>>> staging
  },
  { path: 'directorate', loadChildren: () => import('./directorate/directorate.module').then(m => m.DirectorateModule), data: { title: 'Workflow::Directorate' } },
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule), data: { title: 'Workflow::Dashboard' } },
  { path: 'translation', loadChildren: () => import('./translation/translation.module').then(m => m.TranslationModule), data: { title: 'Workflow::Translation' } },
  { path: 'transcription', loadChildren: () => import('./transcription/transcription.module').then(m => m.TranscriptionModule), data: { title: 'Workflow::Transcription' } },
  { path: 'transcribe', loadChildren: () => import('./transcribe/transcribe.module').then(m => m.TranscribeModule), data: { title: 'Workflow::Transcribe' } },
  { path: 'interpretation', loadChildren: () => import('./interpretation/interpretation.module').then(m => m.InterpretationModule), data: { title: 'Workflow::Interpretation' } },
  { path: 'protocol', loadChildren: () => import('./protocol/protocol.module').then(m => m.ProtocolModule), data: { title: 'Workflow::Protocol' } },
  { path: 'conference', loadChildren: () => import('./conference/conference.module').then(m => m.ConferenceModule), data: { title: 'Workflow::Conference' } },

];

@NgModule({
  imports: [RouterModule.forRoot(routes, {})],
  exports: [RouterModule],
})
export class AppRoutingModule { }
