import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs/internal/firstValueFrom';

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
  public authError = false;
  constructor(private http: HttpClient) {}

  public async init(): Promise<void> {
    if (this.userFetched) return;
    const getUser$ = this.http.get<AppUser>('auth/GetAppUser');
    try {
      this.user = await firstValueFrom(getUser$);
      this.userFetched = true;
    } catch {
      this.authError = true;
    }
  }
}
