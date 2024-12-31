// src/app/models/match.model.ts
export interface Match {
  matchId: number;
  matchNumber: string;
  competition: string;
  matchDate: string;
  periods: number;
  periodDuration: number;
  overtimeDuration: number;
  homeTeam: string;
  awayTeam: string;
}
