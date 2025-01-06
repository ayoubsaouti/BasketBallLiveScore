// src/app/services/foul.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service'


@Injectable({
  providedIn: 'root',
})
export class FoulService {
  private apiUrl = 'https://localhost:7295/api/Foul'; // URL de l'API des fautes

  constructor(private http: HttpClient, private authService: AuthService) { }

  // Enregistrer une faute
  recordFoul(matchId: number, foulDto: { playerId: number, foulType: string, playerName: string, elapsedTime: number }): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.post(`${this.apiUrl}/${matchId}/foul`, foulDto, { headers });
  }
}
