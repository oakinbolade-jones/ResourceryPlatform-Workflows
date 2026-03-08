import { Component } from '@angular/core';

@Component({
  selector: 'app-conference-layout',
  templateUrl: './conference-layout.component.html',
  styleUrl: './conference-layout.component.scss'
})
export class ConferenceLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
