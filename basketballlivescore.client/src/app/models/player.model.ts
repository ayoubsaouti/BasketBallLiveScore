export interface Player {
  playerId: number;   // ID unique du joueur
  name: string;       // Nom du joueur
  firstName: string;  // Prénom du joueur
  number: number;     // Numéro du joueur
  position: string;   // Position du joueur (ex: Attaquant, Défenseur, etc.)
  captain: boolean;   // Si le joueur est capitaine ou non
  isPlaying: boolean; // Si le joueur est actuellement en jeu
  teamId: number;     // ID de l'équipe à laquelle appartient le joueur
}
