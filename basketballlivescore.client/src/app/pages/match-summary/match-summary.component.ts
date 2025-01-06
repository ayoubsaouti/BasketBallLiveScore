import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatchService } from '../../services/match.service';
import { SignalrService } from '../../services/signalr.service'; // Importez le service SignalR
import { MatchFacts } from '../../models/matchFacts.model';
import { Score } from '../../models/score.model';
import { Foul } from '../../models/foul.model';

@Component({
  selector: 'app-match-summary',
  templateUrl: './match-summary.component.html',
  styleUrls: ['./match-summary.component.css']
})
export class MatchSummaryComponent implements OnInit, OnDestroy {
  match: any;
  time: number = 0; // Timer en secondes
  currentQuarter: number = 1;
  actions: any[] = [];
  homeTeamScore: number = 0;
  awayTeamScore: number = 0;
  matchId: number | null = null;
  isMatchFinished: boolean = false;

  constructor(
    private matchService: MatchService,
    private route: ActivatedRoute,
    private signalrService: SignalrService // Injectez le service SignalR
  ) { }

  ngOnInit(): void {
    this.matchId = parseInt(this.route.snapshot.paramMap.get('id') || '', 10);
    if (this.matchId) {
      this.loadMatchData(this.matchId);
      this.loadMatchFacts(this.matchId);
      this.loadElapsedTimer(this.matchId);
    } else {
      console.error('Aucun match ID trouvé');
    }

    // Abonnez-vous aux mises à jour SignalR en temps réel
    this.signalrService.matchUpdate$.subscribe((update) => {
      if (update && update.matchId === this.matchId) {
        this.handleMatchUpdate(update);
      }
    });

    // Abonnez-vous aux mises à jour des scores via SignalR
    this.signalrService.scoreUpdate$.subscribe((update) => {
      if (update && update.matchId === this.matchId) {
        this.homeTeamScore = update.homeTeamScore;
        this.awayTeamScore = update.awayTeamScore;
      }
    });

    // Abonnez-vous aux mises à jour du timer via SignalR
    this.signalrService.timerUpdate$.subscribe((elapsedTime) => {
      this.time = elapsedTime;  // Met à jour le timer en temps réel
    });

    // Abonnez-vous aux mises à jour SignalR en temps réel
    this.signalrService.matchFinishUpdateSource$.subscribe((update) => {
      if (update && update.matchId === this.matchId) {
         this.isMatchFinished = update.isFinished;
      }
    });

    // Abonnez-vous aux mises à jour du quart via SignalR
    this.signalrService.quarterUpdate$.subscribe((update) => {
      if (update && update.matchId === this.matchId) {
        this.currentQuarter = update.currentQuarter;
      }
    });
  }

  ngOnDestroy(): void {
    // Fermer la connexion SignalR lorsque le composant est détruit
    this.signalrService.stopConnection();
  }

  loadMatchData(matchId: number): void {
    this.matchService.getMatchById(matchId).subscribe(
      (data) => {
        this.match = data;
        this.homeTeamScore = this.match.homeTeamScore;
        this.awayTeamScore = this.match.awayTeamScore;
        this.currentQuarter = this.match.currentQuarter;
        this.isMatchFinished = this.match.isFinished;
      },
      (error) => {
        console.error('Erreur lors de la récupération des données du match', error);
      }
    );
  }

  loadElapsedTimer(matchId: number): void {
    this.matchService.getMatchById(matchId).subscribe(
      (data) => {
        if (data && data.elapsedTimer !== undefined) {
          this.time = data.elapsedTimer;
        }
      },
      (error) => {
        console.error('Erreur lors du chargement du temps du match', error);
      }
    );
  }

  loadMatchFacts(matchId: number): void {
    this.matchService.getMatchFacts(matchId).subscribe(
      (data: MatchFacts) => {
        this.actions = []; // Réinitialisation du tableau des actions

        // Ajouter les scores dans le tableau des actions
        data.scores.forEach((score: Score) => {
          const formattedTime = this.formatTimeFromSeconds(score.elapsedTime);
          this.actions.push({
            actionType: 'score',
            text: `${score.playerName} a marqué ${score.points} points au quart ${score.quarter} à ${formattedTime}`,
            elapsedTime: score.elapsedTime,
          });
        });

        // Ajouter les fautes dans le tableau des actions
        data.fouls.forEach((foul: Foul) => {
          const formattedTime = this.formatTimeFromSeconds(foul.elapsedTime);
          this.actions.push({
            actionType: 'foul',
            text: `${foul.playerName} a commis une faute de type ${foul.foulType} au quart ${foul.quarter} à ${formattedTime}`,
            elapsedTime: foul.elapsedTime,
          });
        });

        // Trier les actions par temps écoulé
        this.actions.sort((a, b) => b.elapsedTime - a.elapsedTime);
      },
      (error) => {
        console.error('Erreur lors de la récupération des faits du match', error);
      }
    );
  }

  // Format les secondes en minutes:secondes
  formatTime(seconds: number): string {
    const numSeconds = Number(seconds);  // Convertir en nombre pour éviter NaN
    if (isNaN(numSeconds) || numSeconds < 0) {
      console.error(`Invalid elapsedTime value: ${seconds}`);  // Afficher l'erreur pour débogage
      return '0:00';  // Retourner '0:00' si ce n'est pas un nombre valide
    }

    const minutes = Math.floor(numSeconds / 60);  // Calcul des minutes
    const remainingSeconds = numSeconds % 60;    // Calcul des secondes restantes
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`; // Format: '1:08'
  }

  formatTimeFromSeconds(seconds: number): string {
    const numSeconds = Number(seconds);  // Convertir en nombre pour éviter NaN
    if (isNaN(numSeconds) || numSeconds < 0) {
      return '0:00';  // Retourner '0:00' si ce n'est pas un nombre valide
    }
    const minutes = Math.floor(numSeconds / 60);
    const remainingSeconds = numSeconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
  }

  // Gérer la mise à jour en temps réel du match (score, faute, etc.)
  handleMatchUpdate(update: any): void {
    // Assurez-vous que elapsedTime est un nombre et le formatez
    const formattedTime = this.formatTime(Number(update.elapsedTime)); // Assurez-vous qu'il soit bien un nombre

    if (update.actionType === 'score') {
      // Ajouter un score à l'affichage en temps réel avec le temps formaté
      this.actions.push({
        actionType: 'score',
        text: `${update.message} à ${formattedTime}`,  // Affichage du score avec le temps formaté
        elapsedTime: update.elapsedTime,  // Inclure elapsedTime dans le message
      });
    } else if (update.actionType === 'foul') {
      // Ajouter une faute à l'affichage en temps réel avec le temps formaté
      this.actions.push({
        actionType: 'foul',
        text: `${update.message} à ${formattedTime}`,  // Affichage de la faute avec le temps formaté
        elapsedTime: update.elapsedTime,
      });
    }

    // Trier les actions par temps écoulé
    this.actions.sort((a, b) => b.elapsedTime - a.elapsedTime);
  }
}
