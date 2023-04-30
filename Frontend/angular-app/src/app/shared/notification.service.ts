import { Injectable, signal } from '@angular/core';

export enum NotificationType {
  Error,
  Warning,
  Info,
  Success,
}

export interface AppNotification {
  id: string;
  title?: string;
  message: string;
  read: boolean;
  created: Date;
  type: NotificationType;
}

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  public notificationsSig = signal<AppNotification[]>([]);

  public addNotification(message: string, title?: string, type: NotificationType = NotificationType.Info) {
    const newNotification: AppNotification = {
      id: this.generateNotificationId(),
      title,
      message,
      read: false,
      created: new Date(),
      type,
    };
    this.notificationsSig.mutate((_) => _.push(newNotification));
  }

  public removeNotification(value: AppNotification) {
    this.notificationsSig.update((_) => _.filter((_) => _.id !== value.id));
  }

  private generateNotificationId(): string {
    const randomId = Math.random().toString(36).substring(2);
    return this.notificationsSig().find((_) => _.id === randomId) ? this.generateNotificationId() : randomId;
  }
}
