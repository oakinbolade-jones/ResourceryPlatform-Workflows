import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '@abp/ng.core';
import {
  CUSTOM_HTTP_ERROR_HANDLER_PRIORITY,
  CustomHttpErrorHandlerService,
} from '@abp/ng.theme.shared';
import { ApiErrorLocalizationService } from '../api-error-localization.service';

@Injectable({
  providedIn: 'root',
})
export class GlobalHttpErrorPopupHandlerService implements CustomHttpErrorHandlerService {
  readonly priority = CUSTOM_HTTP_ERROR_HANDLER_PRIORITY.high;

  private lastError: HttpErrorResponse | null = null;

  constructor(
    private apiErrorLocalization: ApiErrorLocalizationService,
    private authService: AuthService
  ) {}

  canHandle(error: unknown): boolean {
    const status = this.resolveStatusCode(error);
    if (status === null) {
      this.lastError = null;
      return false;
    }

    this.lastError = error instanceof HttpErrorResponse ? error : new HttpErrorResponse({
      status,
      error: typeof error === 'object' && error !== null ? (error as { error?: unknown }).error : null,
      statusText:
        typeof error === 'object' && error !== null && typeof (error as { statusText?: unknown }).statusText === 'string'
          ? String((error as { statusText?: unknown }).statusText)
          : undefined,
    });

    return status === 0 || status >= 400;
  }

  execute(): void {
    if (!this.lastError) {
      return;
    }

    const friendlyError = this.apiErrorLocalization.showFriendlyErrorPopupFromHttpError(
      this.lastError,
      'Workflow::Common:ApiError:Default',
      'Something unexpected happened. Please try again in a moment.'
    );

    if (this.lastError.status === 401) {
      setTimeout(() => {
        this.authService.navigateToLogin();
      }, 900);
    }

    // Preserve the latest user-friendly message if future diagnostics are added.
    void friendlyError;
  }

  private resolveStatusCode(error: unknown): number | null {
    if (!error || typeof error !== 'object') {
      return null;
    }

    const candidate = (error as { status?: unknown }).status;
    if (typeof candidate !== 'number' || !Number.isFinite(candidate)) {
      return null;
    }

    return candidate;
  }
}
