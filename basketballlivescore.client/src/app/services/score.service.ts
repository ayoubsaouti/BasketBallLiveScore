// src/app/services/foul.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ScoreService {
  private apiUrl = 'https://localhost:7295/api/Score'; // URL de l'API des fautes

  constructor(private http: HttpClient) { }

  // Enregistrer un score
  recordScore(matchId: number, scoreDto: { playerId: number, points: number, playerName: string, elapsedTime: number }): Observable<any> {
    return this.http.post(`${this.apiUrl}/${matchId}/score`, scoreDto);
  }
}
