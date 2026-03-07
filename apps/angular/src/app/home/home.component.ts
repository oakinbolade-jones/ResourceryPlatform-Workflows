import { AuthService, SessionStateService } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit, OnDestroy {
  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  constructor(
    private authService: AuthService,
    private sessionStateService: SessionStateService
  ) {}

  ngOnInit(): void {
    document.body.classList.add('home-page');
  }

  ngOnDestroy(): void {
    document.body.classList.remove('home-page');
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
}
