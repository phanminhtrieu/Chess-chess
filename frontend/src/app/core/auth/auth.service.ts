import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, map, of, tap, switchMap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly API_URL = 'http://localhost:5200/api/frontend/users';
  
  currentUser = signal<any>(null);
  isProfileLoaded = signal<boolean>(false);

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
    this.isProfileLoaded.set(false);
    return this.http.post(`${this.API_URL}/login`, credentials).pipe(
      switchMap((res: any) => {
        if (res.token) {
          localStorage.setItem('jwt_token', res.token);
          localStorage.setItem('refresh_token', res.refreshToken);
          return this.http.get<any>(`${this.API_URL}/profile`).pipe(
            tap(profile => {
              this.currentUser.set(profile);
              this.isProfileLoaded.set(true);
            }),
            map(() => res)
          );
        }
        this.isProfileLoaded.set(true);
        return of(res);
      })
    );
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.API_URL}/register`, data);
  }

  setRole(role: string): Observable<any> {
    return this.http.post(`${this.API_URL}/set-role`, { role }).pipe(
      switchMap(() => {
        return this.http.get<any>(`${this.API_URL}/profile`).pipe(
          tap(profile => {
            this.currentUser.set(profile);
            this.isProfileLoaded.set(true);
          })
        );
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
        next: (profile) => {
          this.currentUser.set(profile);
          this.isProfileLoaded.set(true);
        },
        error: () => {
          this.refreshSession().subscribe({
            next: () => {
              this.http.get<any>(`${this.API_URL}/profile`).subscribe({
                next: (p) => {
                  this.currentUser.set(p);
                  this.isProfileLoaded.set(true);
                },
                error: () => this.isProfileLoaded.set(true)
              });
            },
            error: () => this.isProfileLoaded.set(true)
          });
        }
      });
    } else {
      this.isProfileLoaded.set(true);
    }
  }

  // Helper for AuthGuard
  onReady(): Observable<boolean> {
    return new Observable(subscriber => {
      const check = () => {
        if (this.isProfileLoaded()) {
          subscriber.next(true);
          subscriber.complete();
        }
      };
      
      // Check immediately
      check();
      
      // If not ready, we could use an effect, but since this is called in a guard, 
      // simple interval or better yet, convert signal to observable
      const interval = setInterval(check, 50);
      return () => clearInterval(interval);
    });
  }
}
