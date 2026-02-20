import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.models';
import { firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'jwt';

  constructor(private http: HttpClient) { }

  async login(req: LoginRequest): Promise<void> {
    const url = `${environment.apiBaseUrl}/api/auth/login`;
    const resp = await firstValueFrom(this.http.post<LoginResponse>(url, req));
    localStorage.setItem(this.tokenKey, resp.token);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
