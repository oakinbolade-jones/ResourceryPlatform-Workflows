import { Component } from '@angular/core';

@Component({
  selector: 'app-support-layout',
  templateUrl: './support-layout.component.html',
  styleUrl: './support-layout.component.scss'
})
export class SupportLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
