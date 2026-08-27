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
  { path: 'webcast', loadChildren: () => import('./webcast/webcast.module').then(m => m.WebcastModule) },
  { path: 'support', loadChildren: () => import('./support/support.module').then(m => m.SupportModule) },
  {
    path: 'account-module',
    loadChildren: () => import('./account/account.module').then(m => m.AccountModule),
  },

  {
    path: 'account/manage',
    component: AccountManageRedirectComponent,
    data: { layout: eLayoutType.empty },
  },
  
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(m => m.AccountModule.forLazy()),
  },
  {
    path: 'identity',
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
  },
  { path: 'directorate', loadChildren: () => import('./directorate/directorate.module').then(m => m.DirectorateModule) },
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
  { path: 'translation', loadChildren: () => import('./translation/translation.module').then(m => m.TranslationModule) },
  { path: 'transcription', loadChildren: () => import('./transcription/transcription.module').then(m => m.TranscriptionModule) },
  { path: 'transcribe', loadChildren: () => import('./transcribe/transcribe.module').then(m => m.TranscribeModule) },
  { path: 'interpretation', loadChildren: () => import('./interpretation/interpretation.module').then(m => m.InterpretationModule) },
  { path: 'protocol', loadChildren: () => import('./protocol/protocol.module').then(m => m.ProtocolModule) },
  { path: 'conference', loadChildren: () => import('./conference/conference.module').then(m => m.ConferenceModule) }

];

@NgModule({
  imports: [RouterModule.forRoot(routes, {})],
  exports: [RouterModule],
})
export class AppRoutingModule {}
