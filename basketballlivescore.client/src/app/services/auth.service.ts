// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

interface RegisterRequest {
  email: string;
  password: string;
}

interface LoginRequest {
  email: string;
  password: string;
}

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:7295/api/auth';  // L'URL de ton backend

  constructor(private http: HttpClient) { }

  // Méthode pour l'inscription
  register(registerData: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, registerData).pipe(
      catchError((error) => {
        console.error('Registration error: ', error);
        throw error;
      })
    );
  }

  // Méthode pour la connexion
  login(loginData: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData).pipe(
      catchError((error) => {
        console.error('Login error: ', error);
        throw error;
      })
    );
  }
}
