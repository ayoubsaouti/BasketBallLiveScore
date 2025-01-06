export interface Foul {
  playerId: number;
  foulType: string;
  quarter: number;
  time: string; // ou Date
  playerName: string;
  elapsedTime: number;  // Le temps écoulé en secondes
}
