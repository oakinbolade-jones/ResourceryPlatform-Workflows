import { Component, Input } from '@angular/core';


@Component({
  selector: 'app-resourcery-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class ResourcerySidebarComponent {
  @Input() collapsed = false;
  @Input() showSearch = false;
}
