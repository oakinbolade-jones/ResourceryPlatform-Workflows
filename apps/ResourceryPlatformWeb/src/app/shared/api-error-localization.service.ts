import { Injectable } from '@angular/core';
import { LocalizationService } from '@abp/ng.core';
import { HttpErrorResponse } from '@angular/common/http';
import { AppPopupService } from './services/app-popup.service';

export interface FriendlyApiError {
  code: string;
  message: string;
  title: string;
}

@Injectable({
  providedIn: 'root',
})
export class ApiErrorLocalizationService {
  constructor(
    private localizationService: LocalizationService,
    private appPopupService: AppPopupService
  ) {}

  async resolveMessageFromResponse(response: Response, defaultKey: string, defaultFallback: string): Promise<string> {
    const friendlyError = await this.resolveFriendlyErrorFromResponse(response, defaultKey, defaultFallback);
    return friendlyError.message;
  }

  resolveNetworkMessage(defaultKey: string, defaultFallback: string): string {
    return this.resolveNetworkFriendlyError(defaultKey, defaultFallback).message;
  }

  resolveStatusMessage(status: number, defaultKey: string, defaultFallback: string): string {
    const key = this.normalizeLocalizationKey(`Workflow::Transcription:ApiError:Status${status}`);
    const localizedStatusMessage = this.localizationService.instant(key);

    if (localizedStatusMessage && localizedStatusMessage !== key) {
      return localizedStatusMessage;
    }

    return this.t(defaultKey, defaultFallback);
  }

  async resolveFriendlyErrorFromResponse(
    response: Response,
    defaultKey: string,
    defaultFallback: string
  ): Promise<FriendlyApiError> {
    const payload = await this.tryReadPayload(response);
    const payloadMessage = this.extractMessage(payload);
    const extractedCode = this.extractErrorCode(payload);

    const statusCode = Number.isFinite(response.status) && response.status > 0 ? String(response.status) : '';
    const fallbackCodeFromText = this.extractSpecialCodeFromText(payloadMessage);
    const code = extractedCode || fallbackCodeFromText || statusCode || 'UNKNOWN';

    const message =
      this.resolveFriendlyMessageByCode(code, defaultKey, defaultFallback, payloadMessage) ||
      payloadMessage ||
      this.resolveNoCodeMessage(defaultKey, defaultFallback);

    return {
      code,
      message,
      title: this.resolveFriendlyTitle(),
    };
  }

  resolveNetworkFriendlyError(defaultKey: string, defaultFallback: string): FriendlyApiError {
    return {
      code: 'NETWORK',
      title: this.resolveFriendlyTitle(),
      message: this.t(
        'Workflow::Common:ApiError:Network',
        'We could not reach the server. Please check your connection and try again.'
      ),
    };
  }

  async showFriendlyErrorPopupFromResponse(
    response: Response,
    defaultKey: string,
    defaultFallback: string
  ): Promise<FriendlyApiError> {
    const friendlyError = await this.resolveFriendlyErrorFromResponse(response, defaultKey, defaultFallback);
    this.appPopupService.show({
      title: friendlyError.title,
      message: friendlyError.message,
      code: friendlyError.code,
      tone: 'error',
      durationMs: 30000,
      homeLinkUrl: '/',
      autoRedirectToHomeOnTimeout: true,
    });

    return friendlyError;
  }

  showFriendlyErrorPopupFromNetwork(defaultKey: string, defaultFallback: string): FriendlyApiError {
    const friendlyError = this.resolveNetworkFriendlyError(defaultKey, defaultFallback);
    this.appPopupService.show({
      title: friendlyError.title,
      message: friendlyError.message,
      code: friendlyError.code,
      tone: 'error',
      durationMs: 30000,
      homeLinkUrl: '/',
      autoRedirectToHomeOnTimeout: true,
    });

    return friendlyError;
  }

  showFriendlyErrorPopupFromUnknown(
    message: string | null | undefined,
    defaultKey: string,
    defaultFallback: string
  ): FriendlyApiError {
    const safeMessage = (message ?? '').trim();
    const friendlyError: FriendlyApiError = {
      code: 'UNKNOWN',
      title: this.resolveFriendlyTitle(),
      message: safeMessage || this.resolveNoCodeMessage(defaultKey, defaultFallback),
    };

    this.appPopupService.show({
      title: friendlyError.title,
      message: friendlyError.message,
      code: friendlyError.code,
      tone: 'error',
      durationMs: 30000,
      homeLinkUrl: '/',
      autoRedirectToHomeOnTimeout: true,
    });

    return friendlyError;
  }

  resolveFriendlyErrorFromHttpError(
    error: HttpErrorResponse,
    defaultKey: string,
    defaultFallback: string
  ): FriendlyApiError {
    const payload = error?.error as unknown;
    const payloadMessage = this.extractMessage(payload);
    const extractedCode = this.extractErrorCode(payload);

    const statusCode = Number.isFinite(error.status) && error.status > 0 ? String(error.status) : '';
    const fallbackCodeFromText = this.extractSpecialCodeFromText(payloadMessage);
    const derivedCode = error.status === 0 ? 'NETWORK' : 'UNKNOWN';
    const code = extractedCode || fallbackCodeFromText || statusCode || derivedCode;

    if (code === 'NETWORK') {
      return this.resolveNetworkFriendlyError(defaultKey, defaultFallback);
    }

    const message =
      this.resolveFriendlyMessageByCode(code, defaultKey, defaultFallback, payloadMessage) ||
      payloadMessage ||
      this.resolveNoCodeMessage(defaultKey, defaultFallback);

    return {
      code,
      message,
      title: this.resolveFriendlyTitle(),
    };
  }

  showFriendlyErrorPopupFromHttpError(
    error: HttpErrorResponse,
    defaultKey: string,
    defaultFallback: string
  ): FriendlyApiError {
    const friendlyError = this.resolveFriendlyErrorFromHttpError(error, defaultKey, defaultFallback);
    this.appPopupService.show({
      title: friendlyError.title,
      message: friendlyError.message,
      code: friendlyError.code,
      tone: friendlyError.code === '403' ? 'warning' : 'error',
      durationMs: 30000,
      homeLinkUrl: '/',
      autoRedirectToHomeOnTimeout: true,
    });

    return friendlyError;
  }

  private resolveFriendlyTitle(): string {
    return this.t('Workflow::Common:ApiError:Title', 'We hit a small issue');
  }

  resolveNoCodeMessage(defaultKey: string, defaultFallback: string): string {
    return this.t(
      'Workflow::Common:ApiError:NoCode',
      this.t(defaultKey, defaultFallback) || 'Something unexpected happened. Please try again in a moment.'
    );
  }

  private resolveFriendlyMessageByCode(
    code: string,
    defaultKey: string,
    defaultFallback: string,
    payloadMessage: string | null
  ): string {
    const normalizedCode = (code ?? '').trim().toUpperCase();

    const map: Record<string, string> = {
      '400': this.t(
        'Workflow::Common:ApiError:Code400',
        'We could not process that request. Please review your input and try again.'
      ),
      '401': this.t(
        'Workflow::Common:ApiError:Code401',
        'Your session may have expired. Please sign in again.'
      ),
      '403': this.t(
        'Workflow::Common:ApiError:Code403',
        'You do not have permission to perform this action.'
      ),
      '404': this.t(
        'Workflow::Common:ApiError:Code404',
        'We could not find what you requested. It may have been moved or removed.'
      ),
      '409': this.t(
        'Workflow::Common:ApiError:Code409',
        'This action conflicted with existing data. Please refresh and try again.'
      ),
      '429': this.t(
        'Workflow::Common:ApiError:Code429',
        'Too many requests at once. Please wait a moment and try again.'
      ),
      '500': this.t(
        'Workflow::Common:ApiError:Code500',
        'Our server ran into a problem. Please try again shortly.'
      ),
      '500.3': this.t(
        'Workflow::Common:ApiError:Code500_3',
        'The service is temporarily unavailable while restarting. Please retry in a few seconds.'
      ),
      '502': this.t(
        'Workflow::Common:ApiError:Code502',
        'A gateway error occurred. Please try again in a moment.'
      ),
      '503': this.t(
        'Workflow::Common:ApiError:Code503',
        'The service is currently unavailable. Please try again shortly.'
      ),
      '504': this.t(
        'Workflow::Common:ApiError:Code504',
        'The request timed out. Please try again.'
      ),
    };

    if (map[normalizedCode]) {
      return map[normalizedCode];
    }

    if (payloadMessage) {
      return payloadMessage;
    }

    if (normalizedCode && normalizedCode !== 'UNKNOWN') {
      return this.resolveStatusMessage(Number(normalizedCode), defaultKey, defaultFallback);
    }

    return this.resolveNoCodeMessage(defaultKey, defaultFallback);
  }

  private extractSpecialCodeFromText(text: string | null): string {
    const value = (text ?? '').trim();
    if (!value) {
      return '';
    }

    const subStatusMatch = value.match(/\b(\d{3}\.\d+)\b/);
    if (subStatusMatch?.[1]) {
      return subStatusMatch[1];
    }

    return '';
  }

  private extractErrorCode(payload: unknown): string {
    if (!payload || typeof payload !== 'object') {
      return '';
    }

    const dictionary = payload as Record<string, unknown>;
    const directCandidates = ['code', 'errorCode', 'statusCode', 'error_code'];
    for (const candidate of directCandidates) {
      const value = dictionary[candidate];
      if (typeof value === 'string' && value.trim()) {
        return value.trim();
      }
      if (typeof value === 'number' && Number.isFinite(value)) {
        return String(value);
      }
    }

    const nestedError = dictionary['error'];
    if (nestedError && typeof nestedError === 'object') {
      const nested = nestedError as Record<string, unknown>;
      const nestedCode = nested['code'];
      if (typeof nestedCode === 'string' && nestedCode.trim()) {
        return nestedCode.trim();
      }
      if (typeof nestedCode === 'number' && Number.isFinite(nestedCode)) {
        return String(nestedCode);
      }
    }

    return '';
  }

  private async tryReadPayload(response: Response): Promise<unknown | null> {
    const contentType = response.headers.get('content-type')?.toLowerCase() ?? '';

    if (contentType.includes('application/json')) {
      try {
        return await response.clone().json();
      } catch {
        return null;
      }
    }

    try {
      const text = (await response.clone().text()).trim();
      return text || null;
    } catch {
      return null;
    }
  }

  private async tryReadPayloadMessage(response: Response): Promise<string | null> {
    const contentType = response.headers.get('content-type')?.toLowerCase() ?? '';

    if (contentType.includes('application/json')) {
      try {
        const payload = await response.clone().json();
        const message = this.extractMessage(payload);
        if (message) {
          return message;
        }
      } catch {
        // Ignore parse errors and continue with status-based message.
      }
    }

    try {
      const text = (await response.clone().text()).trim();
      return text.length > 0 ? text : null;
    } catch {
      return null;
    }
  }

  private extractMessage(payload: unknown): string | null {
    if (typeof payload === 'string') {
      const trimmed = payload.trim();
      return trimmed.length > 0 ? trimmed : null;
    }

    if (!payload || typeof payload !== 'object') {
      return null;
    }

    const dictionary = payload as Record<string, unknown>;
    const candidates = ['message', 'error_description', 'error', 'detail', 'title'];

    for (const candidate of candidates) {
      const value = dictionary[candidate];
      if (typeof value === 'string' && value.trim().length > 0) {
        return value;
      }
    }

    return null;
  }

  private t(key: string, fallback: string): string {
    const normalizedKey = this.normalizeLocalizationKey(key);
    if (!normalizedKey) {
      return fallback;
    }

    const value = this.localizationService.instant(normalizedKey);
    return value && value !== normalizedKey ? value : fallback;
  }

  private normalizeLocalizationKey(key: string): string {
    const trimmed = (key ?? '').trim();
    if (!trimmed) {
      return '';
    }

    return trimmed.includes('::') ? trimmed : `Workflow::${trimmed}`;
  }
}
