// src/app/services/timer.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service'


@Injectable({
  providedIn: 'root',
})
export class TimerService {
  private apiUrl = 'https://localhost:7295/api/Timer'; // URL de l'API Timer

  constructor(private http: HttpClient, private authService: AuthService) { }

  // Mettre à jour le timer
  updateElapsedTimer(matchId: number, elapsedTimer: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.post(`${this.apiUrl}/updateElapsedTimer/${matchId}`, elapsedTimer, { headers });
  }

  // Terminer un match
  finishMatch(matchId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.post(`${this.apiUrl}/finishMatch/${matchId}`, {}, { headers });
  }
}
