import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';

type ThemeMode = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private static readonly storageKey = 'resourceryWorkflow-theme';
  private currentTheme: ThemeMode = 'dark';

  constructor(@Inject(DOCUMENT) private document: Document) {}

  initTheme(): ThemeMode {
    const saved = localStorage.getItem(ThemeService.storageKey) as ThemeMode | null;
    const prefersDark = window.matchMedia?.('(prefers-color-scheme: dark)').matches ?? false;
    const theme = saved ?? (prefersDark ? 'dark' : 'light');
    this.applyTheme(theme, false);
    return this.currentTheme;
  }

  getTheme(): ThemeMode {
    return this.currentTheme;
  }

  toggleTheme(): ThemeMode {
    const next: ThemeMode = this.currentTheme === 'dark' ? 'light' : 'dark';
    this.applyTheme(next, true);
    return this.currentTheme;
  }

  private applyTheme(theme: ThemeMode, persist: boolean): void {
    this.currentTheme = theme;
    this.document.body.setAttribute('data-theme', theme);
    if (persist) {
      localStorage.setItem(ThemeService.storageKey, theme);
    }
  }
}
