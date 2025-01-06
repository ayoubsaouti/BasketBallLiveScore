import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatchService } from '../../services/match.service';
import { FoulService } from '../../services/foul.service';
import { ScoreService } from '../../services/score.service';
import { TimerService } from '../../services/timer.service';

import { MatchFacts } from '../../models/matchFacts.model';
import { Score } from '../../models/score.model';
import { Foul } from '../../models/foul.model';

@Component({
  selector: 'app-encoding-facts',
  templateUrl: './encoding-facts.component.html',
  styleUrls: ['./encoding-facts.component.css']
})
export class EncodingFactsComponent implements OnInit {
  match: any;
  time: number = 0; // Timer en secondes
  timerRunning: boolean = false;
  interval: any;
  currentQuarter: number = 1;

  selectedPlayerTeam1: any;
  selectedPlayerTeam2: any;

  selectedPointsTeam1: number = 1;
  selectedPointsTeam2: number = 1;
  selectedFoulTypeTeam1: string = 'P0';
  selectedFoulTypeTeam2: string = 'P0';

  actions: any[] = [];
  homeTeamScore: number = 0;
  awayTeamScore: number = 0;
  matchId: number | null = null;
  totalMatchTime: number = 0;
  isMatchFinished: boolean = false;

  constructor(private matchService: MatchService, private foulService: FoulService, private scoreService: ScoreService, private timerService: TimerService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.matchId = parseInt(this.route.snapshot.paramMap.get('id') || '', 10);
    if (this.matchId) {
      this.loadMatchData(this.matchId);
      this.loadMatchFacts(this.matchId);
      this.loadElapsedTimer(this.matchId); // Charger le timer actuel

    } else {
      console.error('Aucun match ID trouvé');
    }
  }

  calculateMatchDuration(): void {
    if (this.match && this.match.periodDuration && this.match.periods) {
      // Durée totale en secondes
      this.totalMatchTime = (this.match.periodDuration * 60) * this.match.periods;
      console.log("Durée totale du match en secondes : ", this.totalMatchTime);
    } else {
      console.log('test');
    }
  }
  
  // Charger l'état du timer
  loadElapsedTimer(matchId: number): void {
    this.matchService.getMatchById(matchId).subscribe(
      (data) => {
        if (data && data.elapsedTimer !== undefined) {
          this.time = data.elapsedTimer;  // Récupérer le temps écoulé depuis la DB
        }
      },
      (error) => {
        console.error('Erreur lors du chargement du temps du match', error);
      }
    );
  }
  loadMatchData(matchId: number): void {
    this.matchService.getMatchById(matchId).subscribe(
      (data) => {
        this.match = data;
        this.isMatchFinished = this.match.isMatchFinished; // Assurez-vous de récupérer isMatchFinished
        this.homeTeamScore = this.match.homeTeamScore;  // Mettre à jour le score de l'équipe à domicile
        this.awayTeamScore = this.match.awayTeamScore;  // Mettre à jour le score de l'équipe visiteuse
        this.currentQuarter = this.match.currentQuarter;
        this.calculateMatchDuration();


        console.log('Match récupéré:', this.match);
      },
      (error) => {
        console.error('Erreur lors de la récupération des données du match', error);
      }
    );
  }


  loadMatchFacts(matchId: number): void {
    this.matchService.getMatchFacts(matchId).subscribe(
      (data: MatchFacts) => {
        console.log('Données reçues:', data); // Ajoutez cette ligne pour examiner ce que vous recevez du backend

        this.actions = [];  // Réinitialisation du tableau des actions

        // Ajouter les scores dans le tableau des actions avec leur date et temps écoulé
        data.scores.forEach((score: Score) => {
          // Assurez-vous que elapsedTime est un nombre et qu'il est bien formaté
          const formattedTime = this.formatTimeFromSeconds(score.elapsedTime); // Formater l'heure du score
          console.log("AAAAAAA", score.elapsedTime); // Débogage

          this.actions.push({
            actionType: 'score',
            text: `${score.playerName} a marqué ${score.points} points au quart ${score.quarter} à ${formattedTime}`,
            date: new Date(),  // Utilisation de la date actuelle pour l'ajout
            elapsedTime: score.elapsedTime,  // Ajout du temps écoulé
          });
        });

        // Ajouter les fautes dans le tableau des actions avec leur date et temps écoulé
        data.fouls.forEach((foul: Foul) => {
          // Assurez-vous que elapsedTime est un nombre et qu'il est bien formaté
          const formattedTime = this.formatTimeFromSeconds(foul.elapsedTime); // Formater l'heure de la faute
          console.log("Foul Formatted Time", formattedTime); // Débogage

          this.actions.push({
            actionType: 'foul',
            text: `${foul.playerName} a commis une faute de type ${foul.foulType} au quart ${foul.quarter} à ${formattedTime}`,
            date: new Date(), // Date d'ajout de l'action
            elapsedTime: foul.elapsedTime, // Ajouter le temps écoulé
          });
        });

        // Trier les actions par le temps écoulé (elapsedTime) de manière décroissante (du plus récent au plus ancien)
        this.actions.sort((a, b) => b.elapsedTime - a.elapsedTime);  // Trier par elapsedTime, pas par date

        console.log('Faits du match récupérés:', data);
      },
      (error) => {
        console.error('Erreur lors de la récupération des faits du match', error);
      }
    );
  }




  formatElapsedTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
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
    if (this.isMatchFinished) {
      alert("Le match est terminé, vous ne pouvez plus ajouter de nouveaux événements.");
      return;
    }
    let player, points, playerName;

    if (team === 'team1') {
      player = this.selectedPlayerTeam1;
      points = this.selectedPointsTeam1;
      this.homeTeamScore += points;
    } else if (team === 'team2') {
      player = this.selectedPlayerTeam2;
      points = this.selectedPointsTeam2;
      this.awayTeamScore += points;
    }

    if (player && points && this.matchId !== null) {
      playerName = `${player.firstName} ${player.lastName}`;


     
      // Créer l'objet ScoreDTO avec l'heure du timer
      const scoreDto = {
        playerId: player.playerId,
        points: points,
        playerName: playerName,
        elapsedTime: this.time, // Passer le temps écoulé du timer
        quarter: this.currentQuarter // Ajouter le quart calculé
      };

      this.scoreService.recordScore(this.matchId!, scoreDto).subscribe(
        (response) => {
          console.log('Panier marqué avec succès', response);

          // Recharger les faits du match après l'ajout du panier
          this.loadMatchFacts(this.matchId!);
        },
        (error) => {
          console.error('Erreur lors de l\'enregistrement du panier', error);
        }
      );
    }
  }

  // Enregistrer une faute
  recordFoul(team: string): void {
    if (this.isMatchFinished) {
      alert("Le match est terminé, vous ne pouvez plus ajouter de nouveaux événements.");
      return;
    }
    let player, foulType, playerName;

    if (team === 'team1') {
      player = this.selectedPlayerTeam1;
      foulType = this.selectedFoulTypeTeam1;
    } else if (team === 'team2') {
      player = this.selectedPlayerTeam2;
      foulType = this.selectedFoulTypeTeam2;
    }

    if (player && foulType && this.matchId !== null) {
      playerName = `${player.firstName} ${player.lastName}`;

      const elapsedTime = this.time; // Assurez-vous que elapsedTime est un nombre et que c'est bien la bonne valeur

      const foulDto = {
        playerId: player.playerId,
        foulType: foulType,
        playerName: playerName,
        elapsedTime: elapsedTime, // Passer elapsedTime en tant que nombre
        quarter: this.currentQuarter // Ajouter le quart calculé

      };

      this.foulService.recordFoul(this.matchId!, foulDto).subscribe(
        (response) => {
          this.actions.push({
            actionType: 'foul',
            text: `${player.firstName} ${player.lastName} a commis une faute de type ${foulType} au quart ${foulDto.quarter} à ${this.formatTime(elapsedTime)}`,
            date: new Date(),
            elapsedTime: elapsedTime
          });
          console.log('Faute enregistrée avec succès', response);
        },
        (error) => {
          console.error('Erreur lors de l\'enregistrement de la faute', error);
        }
      );
    }
  }

  // Fonction pour démarrer et arrêter le chrono
  startStopTimer(): void {
    if (this.isMatchFinished) {
      alert("Le match est terminé, vous ne pouvez plus ajouter de nouveaux événements.");
      return;
    }
    if (this.timerRunning) {
      clearInterval(this.interval);
      this.timerRunning = false;
    } else {
      this.interval = setInterval(() => {
        // Vérifiez si le match n'est pas terminé et que le quart actuel n'a pas dépassé
        if (this.time < this.match.periodDuration * 60 * this.match.periods) {
          this.time++;
          this.saveElapsedTimeToDatabase(); // Sauvegarder le temps dans la DB toutes les secondes

          // Vérification pour passer au prochain quart
          if (this.time >= this.match.periodDuration * 60 * this.currentQuarter) {
            this.updateQuarter(); // Passer au quart suivant si nécessaire
          }
        } else {
          // Le match est terminé, arrêter le timer
          clearInterval(this.interval);
          this.timerRunning = false;
          this.finishMatch(); // Fin du match
        }
      }, 1000);
      this.timerRunning = true;
    }
  }

  updateQuarter(): void {
    if (this.currentQuarter < this.match.periods) {
      this.currentQuarter++;  // Passer au quart suivant

      // Appeler l'API pour mettre à jour le quart-temps actuel
      this.matchService.updateQuarter(this.matchId!).subscribe(
        (response) => {
          console.log(response.message);
          this.loadMatchData(this.matchId!); // Recharger les données du match pour mettre à jour le quart-temps
        },
        (error) => {
          console.error('Erreur lors de la mise à jour du quart-temps', error);
        }
      );
    }
  }

  finishMatch(): void {
    // Appeler un service backend pour marquer le match comme terminé
    this.timerService.finishMatch(this.matchId!).subscribe(
      (response) => {
        console.log("Match terminé", response);
        this.isMatchFinished = true;  // Mettez à jour la variable pour indiquer que le match est terminé
        alert("Le match est terminé. Vous ne pouvez plus ajouter d'événements.");
      },
      (error) => {
        console.error("Erreur lors de la mise à jour du statut du match", error);
      }
    );
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
  }

  formatTimeFromSeconds(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
  }

  // Sauvegarder l'état actuel du timer
  saveElapsedTimeToDatabase(): void {
    if (this.matchId !== null) {
      const elapsedTimer = this.time;  // Temps écoulé du timer
      this.timerService.updateElapsedTimer(this.matchId!, elapsedTimer).subscribe(
        (response) => {
          console.log("Temps du timer sauvegardé", response);
        },
        (error) => {
          console.error("Erreur lors de la sauvegarde du temps du timer", error);
        }
      );
      if (elapsedTimer == this.totalMatchTime) {
        this.finishMatch();
      }
    }
  }
}
