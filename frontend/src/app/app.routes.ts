import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'select-role',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/role-selection/role-selection.component').then(m => m.RoleSelectionComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/shell/shell.component').then(m => m.ShellComponent),
    children: [
      {
        path: 'student/dashboard',
        loadComponent: () => import('./pages/student/student-dashboard.component').then(m => m.StudentDashboardComponent)
      },
      {
        path: 'teacher/dashboard',
        loadComponent: () => import('./pages/teacher/teacher-dashboard.component').then(m => m.TeacherDashboardComponent)
      },
      {
        path: 'teacher/assignments/create',
        loadComponent: () => import('./pages/teacher/assignment-create.component').then(m => m.AssignmentCreateComponent)
      }
    ]
  }
];
