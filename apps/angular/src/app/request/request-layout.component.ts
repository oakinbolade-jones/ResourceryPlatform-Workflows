import { Component } from '@angular/core';

@Component({
  selector: 'app-request-layout',
  templateUrl: './request-layout.component.html',
  styleUrl: './request-layout.component.scss'
})
export class RequestLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
