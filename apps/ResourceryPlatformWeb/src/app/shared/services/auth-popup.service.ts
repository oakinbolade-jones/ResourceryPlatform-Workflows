import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { ConfigStateService } from '@abp/ng.core';
import { lastValueFrom } from 'rxjs';

const POPUP_CALLBACK_PATH = '/assets/auth-popup-callback.html';

@Injectable({ providedIn: 'root' })
export class AuthPopupService {
  constructor(
    private oAuthService: OAuthService,
    private configStateService: ConfigStateService
  ) {}

  private get popupRedirectUri(): string {
    return `${window.location.origin}${POPUP_CALLBACK_PATH}`;
  }

  async loginWithPopup(): Promise<void> {
    const oauthService = this.oAuthService as any;
    const originalSilentRefreshRedirectUri = oauthService.silentRefreshRedirectUri;

    try {
      oauthService.silentRefreshRedirectUri = this.popupRedirectUri;
      await this.oAuthService.initLoginFlowInPopup();
      await lastValueFrom(this.configStateService.refreshAppState());
    } finally {
      oauthService.silentRefreshRedirectUri = originalSilentRefreshRedirectUri;
    }
  }
}
