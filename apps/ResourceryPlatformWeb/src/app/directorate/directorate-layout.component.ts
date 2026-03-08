import { Component } from '@angular/core';

@Component({
  selector: 'app-directorate-layout',
  templateUrl: './directorate-layout.component.html',
  styleUrl: './directorate-layout.component.scss'
})
export class DirectorateLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
