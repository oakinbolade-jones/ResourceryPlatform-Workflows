import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface PageHeaderVisibilityState {
  hideTitleBar: boolean;
  hideSubtitle: boolean;
}

const DEFAULT_STATE: PageHeaderVisibilityState = {
  hideTitleBar: false,
  hideSubtitle: false,
};

@Injectable({ providedIn: 'root' })
export class PageHeaderVisibilityService {
  private readonly overrides = new Map<symbol, PageHeaderVisibilityState>();
  private readonly stateSubject = new BehaviorSubject<PageHeaderVisibilityState>(DEFAULT_STATE);

  readonly state$ = this.stateSubject.asObservable();

  register(token: symbol, state: PageHeaderVisibilityState): void {
    this.overrides.set(token, state);
    this.emitCombinedState();
  }

  update(token: symbol, state: PageHeaderVisibilityState): void {
    if (!this.overrides.has(token)) {
      this.register(token, state);
      return;
    }

    this.overrides.set(token, state);
    this.emitCombinedState();
  }

  unregister(token: symbol): void {
    if (!this.overrides.delete(token)) {
      return;
    }

    this.emitCombinedState();
  }

  private emitCombinedState(): void {
    const combinedState: PageHeaderVisibilityState = {
      hideTitleBar: false,
      hideSubtitle: false,
    };

    this.overrides.forEach(state => {
      combinedState.hideTitleBar = combinedState.hideTitleBar || state.hideTitleBar;
      combinedState.hideSubtitle = combinedState.hideSubtitle || state.hideSubtitle;
    });

    this.stateSubject.next(combinedState);
  }
}
