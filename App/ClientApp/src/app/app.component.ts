import { Component, effect } from '@angular/core';
import { environment } from '../environments/environment';
import { AppUser, AuthUserService } from './security/auth-user.service';
import { LoadingService } from './shared/services/loading.service';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  public title = environment.title;
  public loading: boolean = false;
  public user?: AppUser;
  public showSideNav: boolean = true;
  public isSmallScreen: boolean = false;

  constructor(
    private authUserService: AuthUserService,
    private loadingService: LoadingService,
    responsive: BreakpointObserver
  ) {
    this.user = this.authUserService.user;
    effect(() => {
      this.loading = this.loadingService.isLoading();
    });
    responsive.observe([Breakpoints.Small, Breakpoints.XSmall, Breakpoints.Handset]).subscribe((result) => {
      this.isSmallScreen = result.matches;
      this.showSideNav = !result.matches;
    });
  }

  public toggleSideNavCallback() {
    this.showSideNav = !this.showSideNav;
  }
}
