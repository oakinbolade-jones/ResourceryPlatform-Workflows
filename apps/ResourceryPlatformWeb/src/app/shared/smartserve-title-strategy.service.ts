import { Injectable, effect, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Title } from '@angular/platform-browser';
import { RouterStateSnapshot, TitleStrategy } from '@angular/router';
import { LocalizationService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class SmartServeTitleStrategy extends TitleStrategy {
  private readonly title = inject(Title);
  private readonly localizationService = inject(LocalizationService);
  private readonly languageChange = toSignal(this.localizationService.languageChange$);
  private routerState?: RouterStateSnapshot;

  constructor() {
    super();

    effect(() => {
      if (this.languageChange() && this.routerState) {
        this.updateTitle(this.routerState);
      }
    });
  }

  override updateTitle(routerState: RouterStateSnapshot): void {
    this.routerState = routerState;

    const routeTitle = this.buildTitle(routerState);
    if (!routeTitle || routeTitle === 'Workflow::Home') {
      this.title.setTitle('SmartServe Platform - ECOWAS');
      return;
    }

    const resolvedTitle = this.resolveTitle(routeTitle);
    this.title.setTitle(`${resolvedTitle} - SmartServe Platform`);
  }

  private resolveTitle(value: string): string {
    if (!value.startsWith('Workflow::')) {
      return value;
    }

    const localizedValue = this.localizationService.instant({ key: value, defaultValue: value });
    return localizedValue && localizedValue !== value ? localizedValue : value.replace('Workflow::', '');
  }
}