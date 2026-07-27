import { eLayoutType, ReplaceableComponentsService, RoutesService } from '@abp/ng.core';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router, ActivatedRoute } from '@angular/router';
import { Subscription, filter } from 'rxjs';
import { Title } from '@angular/platform-browser';
import { eThemeBasicComponents } from '@abp/ng.theme.basic';
import { LogoComponent } from './resourcery/layout/logo/logo.component';
@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar></abp-loader-bar>
    <abp-dynamic-layout></abp-dynamic-layout>
  `,
})
export class AppComponent implements OnInit, OnDestroy {
  private readonly body = document.body;
  private navigationSub?: Subscription;
  private replaceableComponents = inject(ReplaceableComponentsService);
  private routes = inject(RoutesService);

<<<<<<< HEAD
  private projectTitle = document.title || 'SmartServe';

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private titleService: Title
  ) {  
=======
  constructor(private router: Router) {

    console.log('URL:', window.location.href);
  console.log('QUERY:', window.location.search);
>>>>>>> staging
  }

  ngOnInit(): void {
    this.replaceableComponents.add({
      component: LogoComponent,
      key: eThemeBasicComponents.Logo,
    });

    this.loadTawk();

    this.navigationSub = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(event => {
        const url = (event as NavigationEnd).urlAfterRedirects.split('?')[0];
        
        // Handle home page class
        if (url === '/' || url === '') {
          this.body.classList.add('home-page');
        } else {
          this.body.classList.remove('home-page');
        }

        // Update document title: use route data.title if available, otherwise keep project title
        let route = this.router.routerState.root;
        while (route.firstChild) {
          route = route.firstChild;
        }

        const pageTitle = route.snapshot.data && route.snapshot.data['title'];
        if (pageTitle) {
          this.titleService.setTitle(`${pageTitle} - ${this.projectTitle}`);
        } else {
          this.titleService.setTitle(this.projectTitle);
        }
      });
  }

  ngOnDestroy(): void {
    this.navigationSub?.unsubscribe();
    this.body.classList.remove('home-page');
  }

  private loadTawk(): void {
    if (typeof window === 'undefined') {
      return;
    }

    if ((window as any).Tawk_API || document.getElementById('tawk-script')) {
      return;
    }

    (window as any).Tawk_API = (window as any).Tawk_API || {};
    (window as any).Tawk_LoadStart = new Date();

    const s1 = document.createElement('script');
    s1.id = 'tawk-script';
    s1.async = true;
    s1.src = 'https://embed.tawk.to/69aef567c936f31c351d110a/1jj9mt7mn';
    s1.charset = 'UTF-8';

    const s0 = document.getElementsByTagName('script')[0];
    s0?.parentNode?.insertBefore(s1, s0);
  }
}
