import { Component, OnInit, effect, signal } from '@angular/core';
import { AppNotification, NotificationService, NotificationType } from '../notification.service';

@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.css'],
})
export class NotificationListComponent implements OnInit {
  public notifications = this.notificationService.notificationsSig();

  constructor(private notificationService: NotificationService) {
    effect(() => {
      this.notifications = this.notificationService.notificationsSig();
    });
  }

  public removeNotificationCallback(notification: AppNotification) {
    this.notificationService.removeNotification(notification);
  }

  ngOnInit(): void {
    this.notificationService.addNotification('Hello World 1', 'This is a test 1', NotificationType.Info);
    this.notificationService.addNotification('Hello World 2', 'This is a test 2', NotificationType.Error);
    this.notificationService.addNotification(
      'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut euismod facilisis erat. Pellentesque felis arcu, venenatis et volutpat id, convallis in elit. Etiam erat felis, malesuada in volutpat in, rhoncus in lacus. Donec laoreet est a enim blandit, eget luctus dolor porta. Nulla semper id dui ac blandit. Fusce hendrerit id est quis porta. Maecenas rhoncus diam ipsum, sed scelerisque nibh vehicula et. Donec cursus posuere ligula. Nulla sollicitudin ex aliquam finibus porttitor. Pellentesque tempus sit amet dui a porta. Etiam urna arcu, ultricies sed pretium nec, imperdiet vel lorem. Nunc efficitur urna aliquam, euismod velit nec, efficitur leo. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Maecenas ultrices arcu sem, vitae venenatis libero porttitor a.',
      'This is a test 3',
      NotificationType.Success
    );
    this.notificationService.addNotification(
      'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut euismod facilisis erat. Pellentesque felis arcu, venenatis et volutpat id, convallis in elit. Etiam erat felis, malesuada in volutpat in, rhoncus in lacus. Donec laoreet est a enim blandit, eget luctus dolor porta. Nulla semper id dui ac blandit. Fusce hendrerit id est quis porta. Maecenas rhoncus diam ipsum, sed scelerisque nibh vehicula et. Donec cursus posuere ligula. Nulla sollicitudin ex aliquam finibus porttitor. Pellentesque tempus sit amet dui a porta. Etiam urna arcu, ultricies sed pretium nec, imperdiet vel lorem. Nunc efficitur urna aliquam, euismod velit nec, efficitur leo. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Maecenas ultrices arcu sem, vitae venenatis libero porttitor a.',
      'This is a test 4',
      NotificationType.Warning
    );
  }
}
