import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

const POPUP_CALLBACK_PATH = '/assets/auth-popup-callback.html';
const POPUP_WINDOW_NAME = 'auth-popup';
const POPUP_TIMEOUT_MS = 5 * 60 * 1000; // 5 minutes

/**
 * Handles OpenID Connect authorization-code + PKCE flow entirely inside a popup window.
 *
 * Flow:
 *   1. Patches OAuthService.redirectUri and openUri so that initLoginFlow() builds
 *      the correct auth URL (redirect_uri → popup callback page) and opens it in a popup.
 *   2. The popup navigates to the IdP, the user authenticates, and the IdP redirects
 *      the popup to /assets/auth-popup-callback.html?code=...&state=...
 *   3. auth-popup-callback.html posts { type, href } to window.opener then closes.
 *   4. This service receives the postMessage, extracts the search string, and calls
 *      tryLoginCodeFlow with customHashFragment + customRedirectUri so the token
 *      exchange runs in the parent window using the stored PKCE verifier.
 */
@Injectable({ providedIn: 'root' })
export class AuthPopupService {
  constructor(private oAuthService: OAuthService) {}

  private get popupRedirectUri(): string {
    return `${window.location.origin}${POPUP_CALLBACK_PATH}`;
  }

  loginWithPopup(): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      let popup: Window | null = null;
      let messageHandler: ((event: MessageEvent) => void) | null = null;
      let timeoutHandle: ReturnType<typeof setTimeout> | null = null;
      let pollHandle: ReturnType<typeof setInterval> | null = null;

      const originalRedirectUri = this.oAuthService.redirectUri;
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const originalOpenUri = (this.oAuthService as any).openUri as (uri: string) => void;

      const cleanup = () => {
        if (messageHandler) {
          window.removeEventListener('message', messageHandler);
          messageHandler = null;
        }
        if (timeoutHandle !== null) {
          clearTimeout(timeoutHandle);
          timeoutHandle = null;
        }
        if (pollHandle !== null) {
          clearInterval(pollHandle);
          pollHandle = null;
        }
        // Restore OAuthService to its original state
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        (this.oAuthService as any).openUri = originalOpenUri;
        this.oAuthService.redirectUri = originalRedirectUri;
        if (popup && !popup.closed) {
          popup.close();
        }
        popup = null;
      };

      messageHandler = (event: MessageEvent) => {
        // Strict origin check — only accept messages from the same origin
        if (event.origin !== window.location.origin) return;
        if (!event.data || event.data['type'] !== 'auth-popup-callback') return;

        const callbackHref = event.data['href'] as string;
        cleanup();

        let searchParams: string;
        try {
          searchParams = new URL(callbackHref).search;
        } catch {
          reject(new Error('Auth popup returned an invalid callback URL.'));
          return;
        }

        if (searchParams.includes('error=')) {
          reject(new Error(`Authorization error: ${decodeURIComponent(searchParams)}`));
          return;
        }

        // Exchange the code for tokens in the parent window.
        // customHashFragment → the search string containing code + state
        // customRedirectUri  → must match what was sent in the auth request
        this.oAuthService
          .tryLoginCodeFlow({
            customHashFragment: searchParams,
            customRedirectUri: this.popupRedirectUri,
            preventClearHashAfterLogin: true,
          })
          .then(() => resolve())
          .catch(reject);
      };

      window.addEventListener('message', messageHandler);

      // Override redirectUri so initLoginFlow builds the auth URL with the popup
      // callback page as redirect_uri and stores the matching PKCE state.
      this.oAuthService.redirectUri = this.popupRedirectUri;

      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      (this.oAuthService as any).openUri = (uri: string) => {
        // Restore redirectUri immediately — PKCE state is already saved at this point.
        this.oAuthService.redirectUri = originalRedirectUri;

        popup = window.open(
          uri,
          POPUP_WINDOW_NAME,
          'width=520,height=660,left=200,top=100,resizable=yes,scrollbars=yes,status=no'
        );

        if (!popup) {
          cleanup();
          reject(new Error('The browser blocked the login popup. Please allow popups for this site and try again.'));
          return;
        }

        // Detect if the user closes the popup without completing login
        pollHandle = setInterval(() => {
          if (popup?.closed) {
            cleanup();
            reject(new Error('Login was cancelled.'));
          }
        }, 500);

        // Hard timeout — cleans up if login takes too long
        timeoutHandle = setTimeout(() => {
          cleanup();
          reject(new Error('Login timed out. Please try again.'));
        }, POPUP_TIMEOUT_MS);
      };

      // Triggers PKCE code generation, state saving, and the openUri call above
      try {
        this.oAuthService.initLoginFlow();
      } catch (err) {
        cleanup();
        reject(err);
      }
    });
  }
}
