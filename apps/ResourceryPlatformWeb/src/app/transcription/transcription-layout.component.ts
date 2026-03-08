import { Component } from '@angular/core';

@Component({
  selector: 'app-transcription-layout',
  templateUrl: './transcription-layout.component.html',
  styleUrl: './transcription-layout.component.scss'
})
export class TranscriptionLayoutComponent {
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
