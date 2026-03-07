import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-resourcery-page',
  templateUrl: './page.component.html',
  styleUrl: './page.component.scss',
})
export class ResourceryPageComponent implements OnInit, OnDestroy {
  @Input() collapsed = false;
  @Output() toggleSidebar = new EventEmitter<void>();

  pageTitle = 'Dashboard';
  pageSubtitle = 'Overview';
  hideTopbar = false;

  private readonly destroy$ = new Subject<void>();

  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd), takeUntil(this.destroy$))
      .subscribe(() => this.updateHeader());

    this.updateHeader();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  private updateHeader(): void {
    let activeRoute = this.router.routerState.root;
    while (activeRoute.firstChild) {
      activeRoute = activeRoute.firstChild;
    }

    this.hideTopbar = !!(activeRoute.snapshot.data?.['hideTopbar'] ?? activeRoute.snapshot.data?.['hideHeader']);

    const title = activeRoute.snapshot.data?.['title'] as string | undefined;
    const resolvedTitle = title ?? this.fallbackTitle();
    this.pageTitle = this.stripSpaces(resolvedTitle);
    this.pageSubtitle = `${this.pageTitle}Overview`;
  }

  private stripSpaces(value: string): string {
    return value.replace(/\s+/g, '');
  }

  private fallbackTitle(): string {
    const segment = this.router.url.split('/').filter(Boolean).pop();
    if (!segment) {
      return 'Dashboard';
    }
    return segment.charAt(0).toUpperCase() + segment.slice(1);
  }
}
