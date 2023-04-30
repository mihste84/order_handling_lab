import { Component } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css'],
})
export class LogoutComponent {
  constructor() {
    var fullUrl = environment.apiURL + 'auth/logout?returnUrl=' + environment.appURL;

    window.location.href = fullUrl;
  }
}
