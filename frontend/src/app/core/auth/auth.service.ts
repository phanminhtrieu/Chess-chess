import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, map, of, tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly API_URL = 'http://localhost:5200/api/frontend/users';
  
  currentUser = signal<{ displayName: string, role: string } | null>(null);

  constructor() {
    this.loadProfileIfTokenExists();
  }

  get token(): string | null {
    return localStorage.getItem('jwt_token');
  }

  get refreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.API_URL}/login`, credentials).pipe(
      tap((res: any) => {
        if (res.token) {
          localStorage.setItem('jwt_token', res.token);
          localStorage.setItem('refresh_token', res.refreshToken);
          this.loadProfileIfTokenExists();
        }
      })
    );
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.API_URL}/register`, data);
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
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('refresh_token');
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
          localStorage.setItem('jwt_token', res.token);
          localStorage.setItem('refresh_token', res.refreshToken);
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
        next: (profile) => this.currentUser.set(profile),
        error: () => this.refreshSession().subscribe({
          next: () => {
            this.http.get<any>(`${this.API_URL}/profile`).subscribe(p => this.currentUser.set(p));
          }
        })
      });
    }
  }
}
