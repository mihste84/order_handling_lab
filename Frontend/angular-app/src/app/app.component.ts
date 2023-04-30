import { Component, effect } from '@angular/core';
import { environment } from '../environments/environment';
import { AppUser, AuthUserService } from './shared/auth-user.service';
import { LoadingService } from './shared/loading.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  public title = environment.title;
  public loading: boolean = false;
  public user?: AppUser;

  constructor(
    private authUserService: AuthUserService,
    private loadingService: LoadingService,
    private http: HttpClient
  ) {
    this.user = this.authUserService.user;
    effect(() => {
      this.loading = this.loadingService.isLoading();
    });
  }
}
