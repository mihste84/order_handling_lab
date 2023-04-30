import { Injectable } from '@angular/core';
import { LoggerService, Severity } from './logger.service';
import { NotificationService, NotificationType } from './notification.service';

@Injectable({
  providedIn: 'root',
})
export class ErrorHandlerService {
  constructor(private logger: LoggerService, private notifications: NotificationService) {}

  public addError(value: Error) {
    this.logger.log(value.message, Severity.Error);
    this.notifications.addNotification(value.message, '', NotificationType.Error);
  }
}
