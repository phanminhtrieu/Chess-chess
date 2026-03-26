import { inject } from '@angular/core';
import { map } from 'rxjs';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.onReady().pipe(
    map(() => {
      if (authService.token) {
        const userRole = authService.currentUser()?.role;
        
        // 1. If no role, mandatory redirect to /select-role
        if (!userRole) {
          if (state.url !== '/select-role') {
            return router.createUrlTree(['/select-role']);
          }
          return true;
        }
        
        // 2. If user is on /select-role but already has a role, send them to dashboard
        if (userRole && state.url === '/select-role') {
          return router.createUrlTree([userRole.toLowerCase() + '/dashboard']);
        }

        // 3. Simple RBAC: Prevent students from accessing teacher routes and vice-versa
        if (state.url.startsWith('/teacher/') && userRole !== 'Teacher') {
          return router.createUrlTree(['/student/dashboard']);
        }
        if (state.url.startsWith('/student/') && userRole !== 'Student') {
          return router.createUrlTree(['/teacher/dashboard']);
        }

        // 4. Handle root path redirect based on role
        if (state.url === '/' || state.url === '') {
           return router.createUrlTree([userRole.toLowerCase() + '/dashboard']);
        }

        return true;
      }

      return router.createUrlTree(['/login']);
    })
  );
};
