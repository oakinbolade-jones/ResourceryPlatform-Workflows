import { Component } from '@angular/core';

@Component({
  selector: 'app-interpretation-layout',
  templateUrl: './interpretation-layout.component.html',
  styleUrl: './interpretation-layout.component.scss'
})
export class InterpretationLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
