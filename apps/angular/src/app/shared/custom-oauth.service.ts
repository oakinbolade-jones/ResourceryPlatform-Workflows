import { HttpClient } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { EnvironmentService } from '@abp/ng.core';
import { Params } from '@angular/router';
import { AbpOAuthService } from '@abp/ng.oauth';
import { Observable, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

@Injectable()
export class CustomOAuthService extends AbpOAuthService {
  constructor(
    injector: Injector,
    private readonly httpClient: HttpClient,
    private readonly environmentService: EnvironmentService
  ) {
    super(injector);
  }

  override logout(queryParams?: Params): Observable<any> {
    const issuer = this.environmentService.getEnvironment().oAuthConfig?.issuer || '';
    const customLogoutUrl = `${issuer.replace(/\/$/, '')}/api/custom-logout`;

    return this.httpClient
      .post(customLogoutUrl, {}, { withCredentials: true })
      .pipe(catchError(() => of(null)), switchMap(() => super.logout(queryParams)));
  }
}