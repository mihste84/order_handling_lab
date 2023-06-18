import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, finalize } from 'rxjs';
import { LoadingService } from '../services/loading.service';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
  constructor(private loadingService: LoadingService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const apiReq = request.clone({
      url: 'api/' + request.url,
      headers: request.headers.set('X-Requested-With', 'XMLHttpRequest'),
      withCredentials: true,
    });
    this.loadingService.startLoading();
    return next.handle(apiReq).pipe(finalize(() => this.loadingService.stopLoading()));
  }
}
