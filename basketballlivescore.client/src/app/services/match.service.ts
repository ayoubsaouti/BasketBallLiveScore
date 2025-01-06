// src/app/services/match.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MatchService {
  private apiUrl = 'https://localhost:7295/api/Match'; // URL de l'API de Match

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

  // Récupérer les détails d'un match par ID
  getMatchById(matchId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/getMatch/${matchId}`);
  }

  // Récupérer les faits d'un match
  getMatchFacts(matchId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/getFacts/${matchId}`);
  }

  // Mettre à jour le quart du match
  updateQuarter(matchId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/updateQuarter/${matchId}`, {});
  }
}
