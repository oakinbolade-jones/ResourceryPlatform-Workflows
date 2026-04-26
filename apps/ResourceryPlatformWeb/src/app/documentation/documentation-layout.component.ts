import { Component } from '@angular/core';

@Component({
  selector: 'app-documentation-layout',
  templateUrl: './documentation-layout.component.html',
  styleUrl: './documentation-layout.component.scss'
})
export class DocumentationLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
