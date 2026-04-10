import { eLayoutType } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
      data: { 
        layout: eLayoutType.empty,
        title: 'Home'
      },
  },
  { path: 'request', loadChildren: () => import('./request/request.module').then(m => m.RequestModule) },
  { path: 'resourcery', loadChildren: () => import('./resourcery/resourcery.module').then(m => m.ResourceryModule) },
  
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(m => m.AccountModule.forLazy()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(m => m.IdentityModule.forLazy()),
  },
  {
    path: 'tenant-management',
    loadChildren: () =>
      import('@abp/ng.tenant-management').then(m => m.TenantManagementModule.forLazy()),
  },
  {
    path: 'setting-management',
    loadChildren: () =>
      import('@abp/ng.setting-management').then(m => m.SettingManagementModule.forLazy()),
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
