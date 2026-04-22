import { AuthService, SessionStateService } from '@abp/ng.core';
import { AfterViewInit, Component } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

declare global {
  interface Window {
    Tawk_API?: Record<string, unknown>;
    Tawk_LoadStart?: Date;
  }
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements AfterViewInit {
  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  constructor(
    private authService: AuthService,
    private sessionStateService: SessionStateService,
    private oAuthService: OAuthService
  ) {}

  ngAfterViewInit(): void {
    this.loadTawkWidget();
  }

  async login() {
    const popupLogin = (this.oAuthService as any).initLoginFlowInPopup;

    if (typeof popupLogin === 'function') {
      try {
        await popupLogin.call(this.oAuthService);
        window.location.reload();
        return;
      } catch {
        // Popup blocked/cancelled or popup flow unavailable at runtime.
      }
    }

    const returnUrl = `${window.location.pathname}${window.location.search}${window.location.hash}`;
    this.authService.navigateToLogin({ returnUrl });
  }

  navigateToDashboard() {
    // Implement navigation to the dashboard here
  } 

  changeLanguage(culture: string, event?: Event) {
    event?.preventDefault();
    this.sessionStateService.setLanguage(culture);
  }

  private loadTawkWidget(): void {
    if (typeof document === 'undefined' || typeof window === 'undefined') {
      return;
    }

    const existingScript = document.getElementById('tawkto-script');
    if (existingScript) {
      return;
    }

    window.Tawk_API = window.Tawk_API || {};
    window.Tawk_LoadStart = new Date();

    const script = document.createElement('script');
    script.id = 'tawkto-script';
    script.async = true;
    script.src = 'https://embed.tawk.to/69aef567c936f31c351d110a/1jj9mt7mn';
    script.charset = 'UTF-8';
    script.setAttribute('crossorigin', '*');
    document.head.appendChild(script);
  }
}
