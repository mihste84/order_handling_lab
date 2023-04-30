import { Injectable, signal } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, finalize, startWith } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoadingService } from './loading.service';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
  constructor(private loadingService: LoadingService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const apiReq = request.clone({ url: environment.apiURL + request.url });
    this.loadingService.startLoading();
    return next.handle(apiReq).pipe(finalize(() => this.loadingService.stopLoading()));
  }
}
