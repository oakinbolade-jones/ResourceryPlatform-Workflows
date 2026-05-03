import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@abp/ng.core';
<<<<<<< HEAD
=======
import { AuthPopupService } from '../services/auth-popup.service';
>>>>>>> refs/heads/development

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
<<<<<<< HEAD
  constructor(private authService: AuthService) {}

  canActivate(
    _route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const isAuthenticated = this.authService.isAuthenticated;
    
    if (!isAuthenticated) {
      this.authService.navigateToLogin({ returnUrl: state.url });
      return false;
    }
    
    return true;
=======
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
>>>>>>> refs/heads/development
  }
}
