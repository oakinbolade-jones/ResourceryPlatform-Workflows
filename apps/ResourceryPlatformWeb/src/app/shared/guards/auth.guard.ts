import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private oAuthService: OAuthService
  ) {}

  async canActivate(
    _route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    const isAuthenticated = this.authService.isAuthenticated;
    
    if (!isAuthenticated) {
      const popupLogin = (this.oAuthService as any).initLoginFlowInPopup;
      if (typeof popupLogin === 'function') {
        try {
          await popupLogin.call(this.oAuthService);
          return true;
        } catch {
          // Popup blocked/cancelled or unavailable, fallback to redirect login.
        }
      }

      this.authService.navigateToLogin({ returnUrl: state.url });
      return false;
    }
    
    return true;
  }
}
