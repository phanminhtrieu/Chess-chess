import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { BackofficeAuthService } from './auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(BackofficeAuthService);
  const router = inject(Router);

  if (authService.token) {
    return true;
  }

  return router.createUrlTree(['/login']);
};
