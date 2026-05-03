import { Component } from '@angular/core';

@Component({
  selector: 'app-webcast-layout',
  templateUrl: './webcast-layout.component.html',
  styleUrl: './webcast-layout.component.scss'
})
export class WebcastLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
