// src/app/services/match.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service'

@Injectable({
  providedIn: 'root',
})
export class MatchService {
  private apiUrl = 'https://localhost:7295/api/Match'; // URL de l'API de Match

  constructor(private http: HttpClient, private authService: AuthService) { }

  // Récupérer tous les matchs
  getMatches(): Observable<any[]> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.get<any[]>(`${this.apiUrl}/getMatches`, { headers });
  }

  // Créer un match
  createMatch(matchData: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.post<any>(`${this.apiUrl}/newMatch`, matchData, { headers });
  }

  // Ajouter des équipes au match
  addTeamsToMatch(matchId: number, teamsData: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.post<any>(`${this.apiUrl}/addTeams/${matchId}`, teamsData, { headers });
  }

  // Ajouter des joueurs au match
  addPlayersToMatch(matchId: number, playersData: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.post<any>(`${this.apiUrl}/addPlayers/${matchId}`, playersData, { headers });
  }

  // Récupérer les détails d'un match par ID
  getMatchById(matchId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.get<any>(`${this.apiUrl}/getMatch/${matchId}`, { headers });
  }

  // Récupérer les faits d'un match
  getMatchFacts(matchId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.get<any>(`${this.apiUrl}/getFacts/${matchId}`, { headers });
  }

  // Mettre à jour le quart du match
  updateQuarter(matchId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();  // Utilisation de la méthode pour obtenir les headers

    return this.http.post(`${this.apiUrl}/updateQuarter/${matchId}`, {}, { headers });
  }
}
