import { Component } from '@angular/core';
import { MatchService } from '../../services/match.service';
import { Router } from '@angular/router';

// Définir une interface pour les joueurs
export interface Player {
  firstName: string;
  lastName: string;
  number: number;
  position: string;
  isCaptain: boolean;
  isInGame: boolean;
}

@Component({
  selector: 'app-add-match',
  templateUrl: './add-match.component.html',
  styleUrls: ['./add-match.component.css']
})
export class AddMatchComponent {
  currentStep = 1;  // Étape initiale (1 = Création du match, 2 = Ajouter les équipes, 3 = Ajouter les joueurs)

  matchData = {
    matchNumber: '',
    competition: '',
    matchDate: '',
    periods: '',
    periodDuration: '',
    overtimeDuration: '',
    encoder: ''
  };

  teamsData = {
    homeTeamName: '',
    homeShortName: '',
    homeTeamCode: '',
    homeTeamColor: '',
    awayTeamName: '',
    awayShortName: '',
    awayTeamCode: '',
    awayTeamColor: ''
  };

  // Données des joueurs
  playersData = {
    homePlayers: [] as Player[], // Liste des joueurs à domicile
    awayPlayers: [] as Player[]  // Liste des joueurs visiteurs
  };

  matchId: number | null = null;  // Initialisé à null pour indiquer qu'il n'a pas encore été défini

  // Réinitialisation des champs du formulaire de joueur
  homePlayerForm = {
    firstName: '',
    lastName: '',
    number: 0,
    position: '',
    isCaptain: false,
    isInGame: false
  };

  awayPlayerForm = {
    firstName: '',
    lastName: '',
    number: 0,
    position: '',
    isCaptain: false,
    isInGame: false
  };

  constructor(private matchService: MatchService, private router: Router) { }

  // Étape 1 : Créer un match
  createMatch(): void {
    this.matchService.createMatch(this.matchData).subscribe((match) => {
      this.matchId = match.matchId;  // Récupérer l'ID du match créé
      // Une fois le match créé, passer à l'étape suivante (ajout des équipes)
      this.currentStep = 2;
    });
  }

  // Étape 2 : Ajouter les équipes au match
  addTeamsToMatch(): void {
    if (!this.matchId) {
      console.error("Match ID is missing");
      return;
    }

    this.matchService.addTeamsToMatch(this.matchId, this.teamsData).subscribe(() => {
      // Une fois les équipes ajoutées, passer à l'étape suivante (ajout des joueurs)
      this.currentStep = 3;
    });
  }

  // Étape 3 : Ajouter les joueurs aux équipes
  addPlayersToMatch(): void {
    if (!this.matchId) {
      console.error("Match ID is missing");
      return;
    }

    this.matchService.addPlayersToMatch(this.matchId, this.playersData).subscribe(() => {
      alert('Match créé avec succès!');
      this.router.navigate(['/home']);  // Redirige vers la page de la liste des matchs
    });
  }

  // Ajouter un joueur à domicile
  addHomePlayer(): void {
    if (this.homePlayerForm.firstName && this.homePlayerForm.lastName && this.homePlayerForm.number) {
      this.playersData.homePlayers.push({ ...this.homePlayerForm });
      this.resetHomePlayerForm();  // Réinitialiser le formulaire après ajout
    }
  }

  // Ajouter un joueur visiteur
  addAwayPlayer(): void {
    if (this.awayPlayerForm.firstName && this.awayPlayerForm.lastName && this.awayPlayerForm.number) {
      this.playersData.awayPlayers.push({ ...this.awayPlayerForm });
      this.resetAwayPlayerForm();  // Réinitialiser le formulaire après ajout
    }
  }

  // Réinitialiser les champs du formulaire pour l'équipe à domicile
  resetHomePlayerForm(): void {
    this.homePlayerForm = {
      firstName: '',
      lastName: '',
      number: 0,
      position: '',
      isCaptain: false,
      isInGame: false
    };
  }

  // Réinitialiser les champs du formulaire pour l'équipe visiteuse
  resetAwayPlayerForm(): void {
    this.awayPlayerForm = {
      firstName: '',
      lastName: '',
      number: 0,
      position: '',
      isCaptain: false,
      isInGame: false
    };
  }
}
