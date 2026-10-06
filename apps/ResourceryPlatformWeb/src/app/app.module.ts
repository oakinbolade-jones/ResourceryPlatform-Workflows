import { CoreModule, provideAbpCore, withOptions, withTitleStrategy } from '@abp/ng.core';
import { provideAbpOAuth } from '@abp/ng.oauth';
import { provideSettingManagementConfig } from '@abp/ng.setting-management/config';
import { provideFeatureManagementConfig } from '@abp/ng.feature-management';
import { ThemeSharedModule, provideAbpThemeShared } from '@abp/ng.theme.shared';
import { provideIdentityConfig } from '@abp/ng.identity/config';
import { provideAccountConfig } from '@abp/ng.account/config';
import { provideTenantManagementConfig } from '@abp/ng.tenant-management/config';
import { registerLocale } from '@abp/ng.core/locale';
import { ThemeBasicModule, provideThemeBasicConfig } from '@abp/ng.theme.basic';
import { CUSTOM_ERROR_HANDLERS } from '@abp/ng.theme.shared';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { environment } from '../environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { APP_ROUTE_PROVIDER } from './route.provider';
import { AccountManageRedirectComponent } from './account/account-manage-redirect.component';
import { SmartServeTitleStrategy } from './shared/smartserve-title-strategy.service';
import { ForbiddenHttpErrorHandlerService } from './shared/services/forbidden-http-error-handler.service';
import { GlobalHttpErrorPopupHandlerService } from './shared/services/global-http-error-popup-handler.service';

@NgModule({
  declarations: [AppComponent, AccountManageRedirectComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ThemeSharedModule,
    CoreModule,
    ThemeBasicModule,
  ],
  providers: [
    APP_ROUTE_PROVIDER,

    provideAbpCore(
      withOptions({
        environment,
        registerLocaleFn: registerLocale(),
      }),
      withTitleStrategy(SmartServeTitleStrategy)
    ),
    provideAbpOAuth(),
    provideIdentityConfig(),
    provideSettingManagementConfig(),
    provideFeatureManagementConfig(),
    provideAccountConfig(),
    provideTenantManagementConfig(),
    {
      provide: CUSTOM_ERROR_HANDLERS,
      useExisting: ForbiddenHttpErrorHandlerService,
      multi: true,
    },
    {
      provide: CUSTOM_ERROR_HANDLERS,
      useExisting: GlobalHttpErrorPopupHandlerService,
      multi: true,
    },
    provideAbpThemeShared(),
    provideThemeBasicConfig(),
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
