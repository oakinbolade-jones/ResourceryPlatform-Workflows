import { Directive, Input, OnDestroy } from '@angular/core';
import {
  PageHeaderVisibilityService,
  PageHeaderVisibilityState,
} from '../services/page-header-visibility.service';

export interface HidePageHeaderConfig {
  titleBar?: boolean;
  subtitle?: boolean;
  hideTitleBar?: boolean;
  hideSubtitle?: boolean;
}

type HidePageHeaderInput = boolean | '' | null | undefined | HidePageHeaderConfig;

@Directive({
  selector: '[appHidePageHeader]',
})
export class HidePageHeaderDirective implements OnDestroy {
  private readonly token = Symbol('appHidePageHeader');
  private isRegistered = false;

  @Input('appHidePageHeader')
  set config(value: HidePageHeaderInput) {
    const state = this.normalizeInput(value);

    if (!this.isRegistered) {
      this.visibilityService.register(this.token, state);
      this.isRegistered = true;
      return;
    }

    this.visibilityService.update(this.token, state);
  }

  constructor(private visibilityService: PageHeaderVisibilityService) {}

  ngOnDestroy(): void {
    if (!this.isRegistered) {
      return;
    }

    this.visibilityService.unregister(this.token);
    this.isRegistered = false;
  }

  private normalizeInput(value: HidePageHeaderInput): PageHeaderVisibilityState {
    if (value === '' || value === true) {
      return { hideTitleBar: true, hideSubtitle: true };
    }

    if (value === false || value == null) {
      return { hideTitleBar: false, hideSubtitle: false };
    }

    return {
      hideTitleBar: !!(value.hideTitleBar ?? value.titleBar),
      hideSubtitle: !!(value.hideSubtitle ?? value.subtitle),
    };
  }
}
