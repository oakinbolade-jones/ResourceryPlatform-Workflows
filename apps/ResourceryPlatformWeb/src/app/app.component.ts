import { eLayoutType, ReplaceableComponentsService, RoutesService } from '@abp/ng.core';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subscription, filter } from 'rxjs';
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

  constructor(private router: Router) {  
  }

  ngOnInit(): void {
    this.replaceableComponents.add({
      component: LogoComponent,
      key: eThemeBasicComponents.Logo,
    });

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
      });
  }

  ngOnDestroy(): void {
    this.navigationSub?.unsubscribe();
    this.body.classList.remove('home-page');
  }
}
