import { Component, effect } from '@angular/core';
import { AppNotification, NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.css'],
})
export class NotificationListComponent {
  public notifications = this.notificationService.notificationsSig();

  constructor(private notificationService: NotificationService) {
    effect(() => {
      this.notifications = this.notificationService.notificationsSig();
    });
  }

  public removeNotificationCallback(notification: AppNotification) {
    this.notificationService.removeNotification(notification);
  }
}
