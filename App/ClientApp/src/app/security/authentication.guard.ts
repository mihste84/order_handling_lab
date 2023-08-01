import { inject } from '@angular/core';
import { AuthUserService } from './auth-user.service';
import { Router } from '@angular/router';

export const isAutheticatedGuard = (auth = inject(AuthUserService)) => auth.userFetched && auth.user?.isAuthenticated;
export const redirectAuthenticatedGuard = (router = inject(Router), auth = inject(AuthUserService)) => {
  if (isAutheticatedGuard(auth)) return true;

  return router.parseUrl('/welcome');
};

export const redirectWelcomeGuard = (router = inject(Router), auth = inject(AuthUserService)) => {
  if (!isAutheticatedGuard(auth)) return true;

  return router.parseUrl('/app/home');
};
