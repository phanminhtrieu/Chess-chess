import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of, tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class BackofficeAuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly API_URL = 'http://localhost:5200/api/frontend/users'; // Using frontend auth endpoints for backoffice login
  
  currentUser = signal<{ displayName: string, role: string } | null>(null);

  constructor() {
    this.loadProfileIfTokenExists();
  }

  get token(): string | null {
    return localStorage.getItem('admin_jwt_token');
  }

  get refreshToken(): string | null {
    return localStorage.getItem('admin_refresh_token');
  }

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.API_URL}/login`, credentials).pipe(
      tap((res: any) => {
        if (res.token) {
          localStorage.setItem('admin_jwt_token', res.token);
          localStorage.setItem('admin_refresh_token', res.refreshToken);
          this.loadProfileIfTokenExists();
        }
      })
    );
  }

  logout(): void {
    const rfToken = this.refreshToken;
    if (rfToken) {
      this.http.post(`${this.API_URL}/logout`, { refreshToken: rfToken }).subscribe({
        next: () => this.clearSession(),
        error: () => this.clearSession()
      });
    } else {
      this.clearSession();
    }
  }

  private clearSession(): void {
    localStorage.removeItem('admin_jwt_token');
    localStorage.removeItem('admin_refresh_token');
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  refreshSession(): Observable<any> {
    const rfToken = this.refreshToken;
    if (!rfToken) {
      this.clearSession();
      return of(null);
    }
    return this.http.post(`${this.API_URL}/refresh`, { refreshToken: rfToken }).pipe(
      tap((res: any) => {
        if (res.token) {
          localStorage.setItem('admin_jwt_token', res.token);
          localStorage.setItem('admin_refresh_token', res.refreshToken);
        }
      }),
      catchError(err => {
        this.clearSession();
        throw err;
      })
    );
  }

  private loadProfileIfTokenExists(): void {
    if (this.token) {
      this.http.get<any>(`${this.API_URL}/profile`).subscribe({
        next: (profile) => {
          if (profile.role !== 'Admin') {
            alert('Access Denied. You must be an Admin.');
            this.logout();
          } else {
            this.currentUser.set(profile);
          }
        },
        error: () => this.refreshSession().subscribe({
          next: () => {
            this.http.get<any>(`${this.API_URL}/profile`).subscribe(p => {
              if (p.role === 'Admin') this.currentUser.set(p); else this.logout();
            });
          }
        })
      });
    }
  }
}
