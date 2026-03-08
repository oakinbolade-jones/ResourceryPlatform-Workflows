import { Component } from '@angular/core';

@Component({
  selector: 'app-protocol-layout',
  templateUrl: './protocol-layout.component.html',
  styleUrl: './protocol-layout.component.scss'
})
export class ProtocolLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
