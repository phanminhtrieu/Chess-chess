import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'users', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent) },
  { 
    path: 'users', 
    loadComponent: () => import('./pages/users/user-list.component').then(m => m.UserListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'users/:id',
    loadComponent: () => import('./pages/users/user-detail.component').then(m => m.UserDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: 'users/:id/audit',
    loadComponent: () => import('./pages/users/user-audit.component').then(m => m.UserAuditComponent),
    canActivate: [authGuard]
  }
];
