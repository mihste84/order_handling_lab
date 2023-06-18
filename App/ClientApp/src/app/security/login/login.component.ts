import { Component } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  constructor() {
    var fullUrl = environment.apiURL + 'auth/login?returnUrl=' + window.location.origin;
    window.location.href = fullUrl;
  }
}
