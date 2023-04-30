import { Component, Input } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AppUser } from '../auth-user.service';
import { NgxPopperjsPlacements } from 'ngx-popperjs';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  @Input() public user?: AppUser;
  public popperBottom = NgxPopperjsPlacements.BOTTOMSTART;
  public title = environment.title;
}
