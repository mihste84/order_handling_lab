import { Component, Input } from '@angular/core';
import { NgxPopperjsPlacements } from 'ngx-popperjs';
import { AppUser, AuthUserService } from '../../../security/auth-user.service';
import { environment } from '../../../../environments/environment';
import {
  faLightbulb as faLightbulbSolid,
  faRightToBracket,
  faUser,
  faRightFromBracket,
} from '@fortawesome/free-solid-svg-icons';
import { faLightbulb as faLightbulbRegular } from '@fortawesome/free-regular-svg-icons';
import { NotificationService } from '../../services/notification.service';
import { arrow } from '@popperjs/core';
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  @Input() public user?: AppUser;
  public popperBottom = NgxPopperjsPlacements.BOTTOMSTART;
  public title = environment.title;

  public faRightToBracket = faRightToBracket;
  public faLightbulbSolid = faLightbulbSolid;
  public faLightbulbRegular = faLightbulbRegular;
  public faRightFromBracket = faRightFromBracket;
  public faUser = faUser;

  constructor(public auth: AuthUserService, public notifications: NotificationService) {}
}
