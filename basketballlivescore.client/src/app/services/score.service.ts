// src/app/services/foul.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service'


@Injectable({
  providedIn: 'root',
})
export class ScoreService {
  private apiUrl = 'https://localhost:7295/api/Score'; // URL de l'API des fautes

  constructor(private http: HttpClient, private authService: AuthService) { }

  // Enregistrer un score
  recordScore(matchId: number, scoreDto: { playerId: number, points: number, playerName: string, elapsedTime: number }): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.post(`${this.apiUrl}/${matchId}/score`, scoreDto, { headers });
  }
}
