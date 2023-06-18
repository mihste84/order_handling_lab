import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgxPopperjsModule } from 'ngx-popperjs';

import { WelcomePageComponent } from './components/welcome-page/welcome-page.component';
import { NotFoundPageComponent } from './components/not-found-page/not-found-page.component';
import { NotificationAlertComponent } from './components/notification-alert/notification-alert.component';
import { SideNavComponent } from './components/side-nav/side-nav.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NavLinkComponent } from './components/nav-link/nav-link.component';
import { HeaderComponent } from './components/header/header.component';
import { NotificationCardComponent } from './components/notification-card/notification-card.component';
import { NotificationListComponent } from './components/notification-list/notification-list.component';
import { DynamicSearchComponent } from './components/dynamic-search/dynamic-search.component';
import { FormsModule } from '@angular/forms';
import { TableHeaderComponent } from './components/table-header/table-header.component';
import { TablePaginationComponent } from './components/table-pagination/table-pagination.component';
import { FormErrorComponent } from './components/form-error/form-error.component';
import { FormControlComponent } from './components/form-control/form-control.component';
import { BlackBackgroundComponent } from './components/black-background/black-background.component';
import { ModalComponent } from './components/modal/modal.component';
import { ConfirmComponent } from './components/confirm/confirm.component';

@NgModule({
  declarations: [
    HeaderComponent,
    NotificationListComponent,
    NotificationCardComponent,
    WelcomePageComponent,
    NotFoundPageComponent,
    NotificationAlertComponent,
    SideNavComponent,
    NavLinkComponent,
    DynamicSearchComponent,
    TableHeaderComponent,
    TablePaginationComponent,
    FormErrorComponent,
    FormControlComponent,
    BlackBackgroundComponent,
    ModalComponent,
    ConfirmComponent,
  ],
  imports: [CommonModule, RouterModule, NgxPopperjsModule, FontAwesomeModule, FormsModule],
  exports: [
    HeaderComponent,
    NotificationListComponent,
    NotificationCardComponent,
    NotificationAlertComponent,
    SideNavComponent,
    NavLinkComponent,
    DynamicSearchComponent,
    TableHeaderComponent,
    TablePaginationComponent,
    FormErrorComponent,
    FormControlComponent,
    BlackBackgroundComponent,
    ModalComponent,
    ConfirmComponent,
  ],
})
export class SharedModule {}
