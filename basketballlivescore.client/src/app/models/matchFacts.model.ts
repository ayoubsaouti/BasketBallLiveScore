import { Score } from './score.model'; // Assurez-vous que les chemins sont corrects
import { Foul } from './foul.model';   // Assurez-vous que les chemins sont corrects
import { Time } from '@angular/common';

export interface MatchFacts {
  matchId: number;
  matchNumber: string;
  competition: string;
  matchDate: Date; // ou Date si vous manipulez directement les objets Date
  periods: number;
  periodDuration: number;
  overtimeDuration: number;
  homeTeamName: string;
  awayTeamName: string;
  homeTeamScore: number;
  awayTeamScore: number;
  scores: Score[];  // Utilisation de l'interface Score
  fouls: Foul[];    // Utilisation de l'interface Foul
}
