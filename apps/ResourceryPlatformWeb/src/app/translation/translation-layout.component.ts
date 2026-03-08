import { Component } from '@angular/core';

@Component({
  selector: 'app-translation-layout',
  templateUrl: './translation-layout.component.html',
  styleUrl: './translation-layout.component.scss'
})
export class TranslationLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
