// src/app/services/timer.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TimerService {
  private apiUrl = 'https://localhost:7295/api/Timer'; // URL de l'API Timer

  constructor(private http: HttpClient) { }

  // Mettre à jour le timer
  updateElapsedTimer(matchId: number, elapsedTimer: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/updateElapsedTimer/${matchId}`, elapsedTimer);
  }

  // Terminer un match
  finishMatch(matchId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/finishMatch/${matchId}`, {});
  }
}
