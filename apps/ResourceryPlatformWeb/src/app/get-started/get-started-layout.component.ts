import { Component } from '@angular/core';

@Component({
  selector: 'app-get-started-layout',
  templateUrl: './get-started-layout.component.html',
  styleUrl: './get-started-layout.component.scss'
})
export class GetStartedLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
