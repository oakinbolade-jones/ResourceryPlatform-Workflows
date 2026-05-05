import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-account-manage-redirect',
  template: '',
})
export class AccountManageRedirectComponent implements OnInit {
  ngOnInit(): void {
    const issuer = (environment.oAuthConfig?.issuer || '').replace(/\/$/, '');
    const fallbackReturnUrl = environment.application?.baseUrl || window.location.origin;

    if (!issuer) {
      window.location.replace('/');
      return;
    }

    const targetUrl = `${issuer}/Account/Manage?returnUrl=${encodeURIComponent(fallbackReturnUrl)}`;
    window.location.replace(targetUrl);
  }
}
