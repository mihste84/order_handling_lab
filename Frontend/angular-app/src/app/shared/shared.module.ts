import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { LoginComponent } from './login/login.component';
import { LogoutComponent } from './logout/logout.component';
import { RouterModule } from '@angular/router';
import { NgxPopperjsModule } from 'ngx-popperjs';
import { NotificationListComponent } from './notification-list/notification-list.component';
import { NotificationCardComponent } from './notification-card/notification-card.component';

@NgModule({
  declarations: [
    HeaderComponent,
    LoginComponent,
    LogoutComponent,
    NotificationListComponent,
    NotificationCardComponent,
  ],
  imports: [CommonModule, RouterModule, NgxPopperjsModule],
  exports: [HeaderComponent, LoginComponent, LogoutComponent, NotificationListComponent, NotificationCardComponent],
})
export class SharedModule {}
