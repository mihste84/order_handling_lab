import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs/internal/firstValueFrom';
import { ErrorHandlerService } from './error-handler.service';

export interface AuthContext {
  user?: AppUser;
  userFetched?: boolean;
}

export interface AppUser {
  isAuthenticated: boolean;
  userName?: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthUserService {
  public user?: AppUser;
  public userFetched?: boolean;

  constructor(private http: HttpClient, private errorHandler: ErrorHandlerService) {}

  public async init(): Promise<void> {
    if (this.userFetched) return;
    const getUser$ = this.http.get<AppUser>('auth/GetAppUser', { withCredentials: true });
    try {
      this.user = await firstValueFrom(getUser$);
      this.userFetched = true;
    } catch {}
  }
}
