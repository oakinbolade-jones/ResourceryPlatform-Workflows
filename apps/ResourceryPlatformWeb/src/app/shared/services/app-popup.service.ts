import { Injectable } from '@angular/core';

export type AppPopupTone = 'error' | 'warning' | 'success' | 'info';

export interface AppPopupOptions {
  title: string;
  message: string;
  tone?: AppPopupTone;
  code?: string;
  logoUrl?: string;
  durationMs?: number;
  showHomeLink?: boolean;
  homeLinkLabel?: string;
  homeLinkUrl?: string;
  autoRedirectToHomeOnTimeout?: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class AppPopupService {
  private readonly containerId = 'ss-app-popup-stack';
  private readonly defaultDurationMs = 30000;
  private readonly defaultLogoUrl = '/assets/images/logo/ecw-smartserve-logo.png';
  private readonly defaultHomeLinkUrl = '/';
  private readonly defaultHomeLinkLabel = 'Go to Home';
  private readonly disableAutoRedirectForDebug = false;

  show(options: AppPopupOptions): void {
    const container = this.ensureContainer();
    const popup = document.createElement('div');
    popup.className = `ss-app-popup ss-app-popup-${options.tone ?? 'info'}`;
    let hasUserInteracted = false;

    const closeButton = document.createElement('button');
    closeButton.type = 'button';
    closeButton.className = 'ss-app-popup-close';
    closeButton.setAttribute('aria-label', 'Close notification');
    closeButton.textContent = 'x';

    const brand = document.createElement('div');
    brand.className = 'ss-app-popup-brand';

    const logo = document.createElement('img');
    logo.className = 'ss-app-popup-logo';
    logo.alt = 'SmartServe';
    logo.src = options.logoUrl?.trim() || this.defaultLogoUrl;
    logo.loading = 'lazy';
    brand.appendChild(logo);

    const content = document.createElement('div');
    content.className = 'ss-app-popup-content';

    const title = document.createElement('div');
    title.className = 'ss-app-popup-title';
    title.textContent = options.title;

    const message = document.createElement('div');
    message.className = 'ss-app-popup-message';
    message.textContent = options.message;

    content.appendChild(title);

    if (options.code?.trim()) {
      const code = document.createElement('div');
      code.className = 'ss-app-popup-code';
      code.textContent = `Error code: ${options.code.trim()}`;
      content.appendChild(code);
    }

    content.appendChild(message);

    const shouldShowHomeLink =
      options.showHomeLink ?? (options.tone === 'error' || options.tone === 'warning');

    if (shouldShowHomeLink) {
      const actions = document.createElement('div');
      actions.className = 'ss-app-popup-actions';

      const homeLink = document.createElement('a');
      homeLink.className = 'ss-app-popup-home-link';
      homeLink.href = options.homeLinkUrl?.trim() || this.defaultHomeLinkUrl;
      homeLink.textContent = options.homeLinkLabel?.trim() || this.defaultHomeLinkLabel;
      homeLink.addEventListener('click', () => {
        hasUserInteracted = true;
      });
      actions.appendChild(homeLink);

      content.appendChild(actions);
    }

    popup.appendChild(closeButton);
    popup.appendChild(brand);
    popup.appendChild(content);
    container.prepend(popup);

    const dismiss = () => {
      popup.classList.add('ss-app-popup-hide');
      setTimeout(() => popup.remove(), 220);
    };

    closeButton.addEventListener('click', () => {
      hasUserInteracted = true;
      dismiss();
    });

    const shouldRedirectOnTimeout =
      options.autoRedirectToHomeOnTimeout ?? (options.tone === 'error' || options.tone === 'warning');
    const redirectUrl = options.homeLinkUrl?.trim() || this.defaultHomeLinkUrl;

    setTimeout(() => {
      dismiss();

      if (!this.disableAutoRedirectForDebug && !hasUserInteracted && shouldRedirectOnTimeout) {
        window.location.assign(redirectUrl);
      }
    }, Math.max(2500, options.durationMs ?? this.defaultDurationMs));
  }

  private ensureContainer(): HTMLElement {
    const existing = document.getElementById(this.containerId);
    if (existing) {
      return existing;
    }

    const container = document.createElement('div');
    container.id = this.containerId;
    container.className = 'ss-app-popup-stack';
    document.body.appendChild(container);
    return container;
  }
}
