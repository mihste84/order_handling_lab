import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  faAnglesLeft,
  faAnglesRight,
  faHouse,
  faFileInvoice,
  faIdCard,
  faDatabase,
} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-side-nav',
  templateUrl: './side-nav.component.html',
  styleUrls: ['./side-nav.component.css'],
})
export class SideNavComponent {
  @Input({ required: true }) public showSideNav?: boolean;
  @Input({ required: true }) public isSmallScreen?: boolean;
  @Output() public onToggleSideNav = new EventEmitter<void>();

  public faAnglesRight = faAnglesRight;
  public faAnglesLeft = faAnglesLeft;
  public faHouse = faHouse;
  public faFileInvoice = faFileInvoice;
  public faIdCard = faIdCard;
  public faDatabase = faDatabase;
}
