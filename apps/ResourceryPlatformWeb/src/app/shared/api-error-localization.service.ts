import { Injectable } from '@angular/core';
import { LocalizationService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class ApiErrorLocalizationService {
  constructor(private localizationService: LocalizationService) {}

  async resolveMessageFromResponse(response: Response, defaultKey: string, defaultFallback: string): Promise<string> {
    const payloadMessage = await this.tryReadPayloadMessage(response);
    if (payloadMessage) {
      return payloadMessage;
    }

    return this.resolveStatusMessage(response.status, defaultKey, defaultFallback);
  }

  resolveNetworkMessage(defaultKey: string, defaultFallback: string): string {
    return this.t('Workflow::Transcription:ApiError:Network', this.t(defaultKey, defaultFallback));
  }

  resolveStatusMessage(status: number, defaultKey: string, defaultFallback: string): string {
    const key = `Workflow::Transcription:ApiError:Status${status}`;
    const localizedStatusMessage = this.localizationService.instant(key);

    if (localizedStatusMessage && localizedStatusMessage !== key) {
      return localizedStatusMessage;
    }

    return this.t(defaultKey, defaultFallback);
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
    const value = this.localizationService.instant(key);
    return value && value !== key ? value : fallback;
  }
}
