import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  CUSTOM_HTTP_ERROR_HANDLER_PRIORITY,
  CustomHttpErrorHandlerService,
} from '@abp/ng.theme.shared';
import { ApiErrorLocalizationService } from '../api-error-localization.service';
import { AppPopupService } from './app-popup.service';

@Injectable({
  providedIn: 'root',
})
export class ForbiddenHttpErrorHandlerService implements CustomHttpErrorHandlerService {
  readonly priority = CUSTOM_HTTP_ERROR_HANDLER_PRIORITY.veryHigh;

  private lastError: unknown = null;

  constructor(
    private apiErrorLocalization: ApiErrorLocalizationService,
    private appPopupService: AppPopupService
  ) {}

  canHandle(error: unknown): boolean {
    this.lastError = error;
    return this.resolveStatusCode(error) === 403;
  }

  execute(): void {
    const fallback = 'You do not have permission to access this page or perform this action.';
    const friendlyMessage = this.apiErrorLocalization.resolveStatusMessage(
      403,
      'Workflow::Common:ApiError:Code403',
      fallback
    );

    const detailsFromPayload = this.extractDetails(this.lastError);

    this.appPopupService.show({
      title: 'Access restricted',
      message: friendlyMessage || detailsFromPayload || fallback,
      code: '403',
      tone: 'warning',
      homeLinkUrl: '/',
      durationMs: 30000,
      autoRedirectToHomeOnTimeout: true,
    });
  }

  private resolveStatusCode(error: unknown): number {
    if (!error || typeof error !== 'object') {
      return 0;
    }

    const candidate = (error as { status?: unknown }).status;
    return typeof candidate === 'number' && Number.isFinite(candidate) ? candidate : 0;
  }

  private extractDetails(error: unknown): string {
    if (!error) {
      return '';
    }

    const source =
      typeof error === 'object' && error !== null
        ? (error as { error?: unknown }).error
        : null;

    if (typeof source === 'string') {
      return source.trim();
    }

    if (!source || typeof source !== 'object') {
      return '';
    }

    const dictionary = source as Record<string, unknown>;
    const topLevelCandidates = ['message', 'error_description', 'detail', 'title'];

    for (const candidate of topLevelCandidates) {
      const value = dictionary[candidate];
      if (typeof value === 'string' && value.trim()) {
        return value.trim();
      }
    }

    const nestedError = dictionary['error'];
    if (nestedError && typeof nestedError === 'object') {
      const nested = nestedError as Record<string, unknown>;
      const nestedCandidates = ['message', 'details', 'detail', 'description'];

      for (const candidate of nestedCandidates) {
        const value = nested[candidate];
        if (typeof value === 'string' && value.trim()) {
          return value.trim();
        }
      }
    }

    return '';
  }
}
