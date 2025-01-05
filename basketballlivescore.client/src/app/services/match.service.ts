// src/app/services/match.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MatchService {
  private apiUrl = 'https://localhost:7295/api/match'; // L'URL de votre API

  constructor(private http: HttpClient) { }

  // Récupérer tous les matchs
  getMatches(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/getMatches`);
  }

  // Créer un match
  createMatch(matchData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/newMatch`, matchData);
  }

  // Ajouter des équipes au match
  addTeamsToMatch(matchId: number, teamsData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/addTeams/${matchId}`, teamsData);
  }

  // Ajouter des joueurs au match
  addPlayersToMatch(matchId: number, playersData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/addPlayers/${matchId}`, playersData);
  }

  // Méthode pour obtenir un match par ID
  getMatchById(matchId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/getMatch/${matchId}`);  // Appel à l'API backend
  }

  // Méthode pour récupérer les faits du match
  getMatchFacts(matchId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/getFacts/${matchId}`);  // Appel à l'API pour obtenir les faits du match
  }

  // Enregistrer un panier marqué
  recordScore(matchId: number, scoreDto: { playerId: number, points: number, playerName: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/${matchId}/score`, scoreDto);
  }

  // Enregistrer une faute
  recordFoul(matchId: number, foulDto: { playerId: number, foulType: string, playerName: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/${matchId}/foul`, foulDto);
  }
}
