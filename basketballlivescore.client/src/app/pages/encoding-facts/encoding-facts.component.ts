import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatchService } from '../../services/match.service';

@Component({
  selector: 'app-encoding-facts',
  templateUrl: './encoding-facts.component.html',
  styleUrls: ['./encoding-facts.component.css']
})
export class EncodingFactsComponent implements OnInit {
  match: any;
  time: number = 0;
  timerRunning: boolean = false;
  interval: any;
  currentQuarter: number = 1;

  selectedPlayerTeam1: any;
  selectedPlayerTeam2: any;


  selectedPointsTeam1: number = 1;
  selectedPointsTeam2: number = 1;
  selectedFoulTypeTeam1: string = 'P0';
  selectedFoulTypeTeam2: string = 'P0';

  actions: string[] = [];
  homeTeamScore: number = 0;
  awayTeamScore: number = 0;
  matchId: number | null = null;

  constructor(private matchService: MatchService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.matchId = parseInt(this.route.snapshot.paramMap.get('id') || '', 10);
    if (this.matchId) {
      this.loadMatchData(this.matchId);
    } else {
      console.error('Aucun match ID trouvé');
    }
  }

  loadMatchData(matchId: number): void {
    this.matchService.getMatchById(matchId).subscribe(
      (data) => {
        this.match = data;
        console.log('Match récupéré:', this.match);
      },
      (error) => {
        console.error('Erreur lors de la récupération des données du match', error);
      }
    );
  }

  // Ajouter un point (1, 2, ou 3)
  addPoints(points: number, team: string): void {
    if (team === 'team1') {
      this.selectedPointsTeam1 = points;
    } else if (team === 'team2') {
      this.selectedPointsTeam2 = points;
    }
  }

  // Sélectionner le type de faute
  selectFoulType(type: string, team: string): void {
    if (team === 'team1') {
      this.selectedFoulTypeTeam1 = type;
    } else if (team === 'team2') {
      this.selectedFoulTypeTeam2 = type;
    }
  }

  // Enregistrer un panier marqué
  recordScore(team: string): void {
    let player, points;
    if (team === 'team1') {
      player = this.selectedPlayerTeam1;
      points = this.selectedPointsTeam1;
      this.homeTeamScore += points; // Mise à jour du score de l'équipe 1
    } else if (team === 'team2') {
      player = this.selectedPlayerTeam2;
      points = this.selectedPointsTeam2;
      this.awayTeamScore += points; // Mise à jour du score de l'équipe 2
    }

    if (player && points && this.matchId !== null) {
      const scoreDto = { playerId: player.playerId, points };
      this.matchService.recordScore(this.matchId!, scoreDto).subscribe(
        (response) => {
          this.actions.push(`${player.firstName} ${player.lastName} a marqué un panier de ${points} points.`);
          console.log('Panier marqué avec succès', response);
        },
        (error) => {
          console.error('Erreur lors de l\'enregistrement du panier', error);
        }
      );
    }
  }

  // Enregistrer une faute
  recordFoul(team: string): void {
    let player, foulType;
    if (team === 'team1') {
      player = this.selectedPlayerTeam1;
      foulType = this.selectedFoulTypeTeam1;
    } else if (team === 'team2') {
      player = this.selectedPlayerTeam2;
      foulType = this.selectedFoulTypeTeam2;
    }

    if (player && foulType && this.matchId !== null) {
      const foulDto = { playerId: player.playerId, foulType, time: '12:41' };
      this.matchService.recordFoul(this.matchId!, foulDto).subscribe(
        (response) => {
          this.actions.push(`${player.firstName} ${player.lastName} a commis une faute de type ${foulType}.`);
          console.log('Faute enregistrée avec succès', response);
        },
        (error) => {
          console.error('Erreur lors de l\'enregistrement de la faute', error);
        }
      );
    } else {
      console.error(player);
    }
  }

  // Fonction pour démarrer et arrêter le chrono
  startStopTimer(): void {
    if (this.timerRunning) {
      clearInterval(this.interval);
      this.timerRunning = false;
    } else {
      this.interval = setInterval(() => {
        this.time++;
      }, 1000);
      this.timerRunning = true;
    }
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
  }
}
