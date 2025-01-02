import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatchService } from '../../services/match.service';

@Component({
  selector: 'app-encoding-facts',
  templateUrl: './encoding-facts.component.html',
  styleUrls: ['./encoding-facts.component.css']
})
export class EncodingFactsComponent implements OnInit {
  match: any;  // Contiendra les données du match récupérées de l'API
  time: number = 0;
  timerRunning: boolean = false;
  interval: any;
  currentQuarter: number = 1;

  selectedPlayer1: any;
  selectedPlayer2: any;
  selectedScoringPlayer: any;
  selectedPoints: number = 1;
  selectedFoulPlayer: any;
  selectedFoulType: string = 'P0';
  selectedSubInPlayer: any;
  selectedSubOutPlayer: any;
  teamWithBall: string | null = null;

  constructor(
    private matchService: MatchService,  // Injection du service MatchService
    private route: ActivatedRoute       // Pour récupérer l'ID du match depuis l'URL
  ) { }

  ngOnInit(): void {
    const matchId = parseInt(this.route.snapshot.paramMap.get('id') || '', 10);  // Récupérer l'ID du match depuis l'URL
    if (matchId) {
      this.loadMatchData(matchId);  // Charger les données du match
    }
  }

  // Charger les données du match
  loadMatchData(matchId: number): void {
    this.matchService.getMatchById(matchId).subscribe(
      (data) => {
        this.match = data;  // Stocke les données récupérées du match
        console.log('Match récupéré:', this.match);  // Pour déboguer
      },
      (error) => {
        console.error('Erreur lors de la récupération des données du match', error);
      }
    );
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

  // Fonction pour changer le quart-temps
  changeQuarter(quarter: number): void {
    this.currentQuarter = quarter;
  }

  // Enregistrer un panier marqué
  recordScore(): void {
    console.log(`Panier marqué par ${this.selectedScoringPlayer.name}: ${this.selectedPoints} points`);
  }

  // Enregistrer une faute
  recordFoul(): void {
    console.log(`Faute de ${this.selectedFoulPlayer.name} : Type ${this.selectedFoulType}`);
  }

  // Enregistrer un changement de joueur
  recordSubstitution(): void {
    console.log(`Changement de joueurs : ${this.selectedSubOutPlayer.name} sort, ${this.selectedSubInPlayer.name} entre`);
  }

  // Enregistrer un time-out
  recordTimeout(): void {
    console.log('Enregistrement d\'un time-out');
  }
}
