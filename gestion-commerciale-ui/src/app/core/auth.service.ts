import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { API_URL } from './api.config';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  expiresAt: string;
}

const TOKEN_KEY = 'gc_token';
const USER_KEY = 'gc_user';
const EXPIRES_KEY = 'gc_expires';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly tokenSignal = signal<string | null>(localStorage.getItem(TOKEN_KEY));

  readonly username = signal<string | null>(localStorage.getItem(USER_KEY));
  readonly isLoggedIn = computed(() => this.tokenSignal() !== null);

  get token(): string | null {
    return this.tokenSignal();
  }

  login(request: LoginRequest) {
    return this.http.post<LoginResponse>(`${API_URL}/auth/login`, request).pipe(
      tap((response) => {
        localStorage.setItem(TOKEN_KEY, response.token);
        localStorage.setItem(USER_KEY, response.username);
        localStorage.setItem(EXPIRES_KEY, response.expiresAt);
        this.tokenSignal.set(response.token);
        this.username.set(response.username);
      })
    );
  }

  logout(): void {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    const expiresAt = localStorage.getItem(EXPIRES_KEY);
    const expired = !expiresAt || new Date(expiresAt) <= new Date();

    if (!this.tokenSignal() || expired) {
      this.clearSession();
      return false;
    }

    return true;
  }

  private clearSession(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(EXPIRES_KEY);
    this.tokenSignal.set(null);
    this.username.set(null);
  }
}