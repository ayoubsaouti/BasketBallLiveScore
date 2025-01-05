export interface Score {
  playerId: number;
  points: number;
  quarter: number;
  time: string; // ou Date si vous gérez le format de la date
  playerName: string;
}
