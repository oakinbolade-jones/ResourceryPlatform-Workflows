import { AuthService, SessionStateService } from '@abp/ng.core';
import { AfterViewInit, Component } from '@angular/core';
<<<<<<< HEAD
=======
import { AuthPopupService } from '../shared/services/auth-popup.service';
>>>>>>> refs/heads/development

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
<<<<<<< HEAD
    private sessionStateService: SessionStateService
=======
    private sessionStateService: SessionStateService,
    private authPopupService: AuthPopupService
>>>>>>> refs/heads/development
  ) {}

  ngAfterViewInit(): void {
    this.loadTawkWidget();
  }

<<<<<<< HEAD
  login() {
    this.authService.navigateToLogin();
=======
  async login() {
    try {
      await this.authPopupService.loginWithPopup();
      window.location.reload();
    } catch {
      // Popup was blocked or cancelled — fall back to full-page redirect.
      const returnUrl = `${window.location.pathname}${window.location.search}${window.location.hash}`;
      this.authService.navigateToLogin({ returnUrl });
    }
>>>>>>> refs/heads/development
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
