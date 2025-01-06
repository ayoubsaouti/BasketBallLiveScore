// src/app/services/foul.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FoulService {
  private apiUrl = 'https://localhost:7295/api/Foul'; // URL de l'API des fautes

  constructor(private http: HttpClient) { }

  // Enregistrer une faute
  recordFoul(matchId: number, foulDto: { playerId: number, foulType: string, playerName: string, elapsedTime: number }): Observable<any> {
    return this.http.post(`${this.apiUrl}/${matchId}/foul`, foulDto);
  }
}
