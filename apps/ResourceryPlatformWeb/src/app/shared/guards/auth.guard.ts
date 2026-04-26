import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { AuthPopupService } from '../services/auth-popup.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private authPopupService: AuthPopupService
  ) {}

  async canActivate(
    _route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    if (this.authService.isAuthenticated) {
      return true;
    }

    try {
      await this.authPopupService.loginWithPopup();
      return true;
    } catch {
      // Popup blocked, cancelled, or timed out — fall back to redirect login.
      this.authService.navigateToLogin({ returnUrl: state.url });
      return false;
    }
  }
}
