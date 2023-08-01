import { Component, Input } from '@angular/core';
import { IconProp } from '@fortawesome/fontawesome-svg-core';

@Component({
  selector: 'app-nav-link',
  templateUrl: './nav-link.component.html',
  styleUrls: ['./nav-link.component.css'],
})
export class NavLinkComponent {
  @Input({ required: true }) public link?: string;
  @Input({ required: true }) public linkText?: string;
  @Input({ required: true }) public icon?: IconProp;
  @Input({ required: true }) public showSideNav?: boolean;
}
