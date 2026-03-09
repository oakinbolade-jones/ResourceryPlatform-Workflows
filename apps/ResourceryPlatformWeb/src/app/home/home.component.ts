import { AuthService, SessionStateService } from '@abp/ng.core';
import { AfterViewInit, Component } from '@angular/core';

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
    private sessionStateService: SessionStateService
  ) {}

  ngAfterViewInit(): void {
    this.loadTawkWidget();
  }

  login() {
    this.authService.navigateToLogin();
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
