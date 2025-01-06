import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';  // Importer HttpHeaders ici
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

  // Stocker le token dans le localStorage
  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  // Récupérer le token du localStorage
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // Supprimer le token du localStorage (déconnexion)
  removeToken(): void {
    localStorage.removeItem('token');
  }

  // Décoder le token JWT et récupérer les informations du rôle
  getRole(): string {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken = this.decodeToken(token);
      return decodedToken.role;
    }
    return '';
  }

  // Décoder le token JWT
  decodeToken(token: string) {
    const payload = token.split('.')[1];
    const decoded = atob(payload);
    return JSON.parse(decoded);
  }

  // Vérifier si l'utilisateur a un rôle spécifique
  hasRole(role: string): boolean {
    console.log(role);
    return this.getRole() === role;
  }

  // Vérifier si l'utilisateur est connecté
  isLoggedIn(): boolean {
    const token = this.getToken();
    return token !== null; // Si un token est présent, l'utilisateur est connecté
  }

  // Méthode pour obtenir les headers avec Authorization Bearer
  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }
}
