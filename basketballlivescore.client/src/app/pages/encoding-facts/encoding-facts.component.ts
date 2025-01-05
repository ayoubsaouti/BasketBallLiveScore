import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatchService } from '../../services/match.service';
import { MatchFacts } from '../../models/matchFacts.model'; // Assurez-vous que le chemin est correct
import { Score } from '../../models/score.model'; // Assurez-vous que le chemin est correct
import { Foul } from '../../models/foul.model'; // Assurez-vous que le chemin est correct


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

  actions: any[] = [];
  homeTeamScore: number = 0;
  awayTeamScore: number = 0;
  matchId: number | null = null;

  constructor(private matchService: MatchService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.matchId = parseInt(this.route.snapshot.paramMap.get('id') || '', 10);
    if (this.matchId) {
      this.loadMatchData(this.matchId);
      this.loadMatchFacts(this.matchId); // Ajouter cette ligne pour charger les faits du match
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

  // Charger les faits du match
  // Charger les faits du match
  loadMatchFacts(matchId: number): void {
    this.matchService.getMatchFacts(matchId).subscribe(
      (data: MatchFacts) => {
        this.actions = [];  // Réinitialisation du tableau des actions

        // Ajouter les scores dans le tableau des actions avec leur date
        data.scores.forEach((score: Score) => {
          const formattedTime = this.formatDate(score.time); // Formater l'heure du score
          this.actions.push({
            actionType: 'score', 
            text: `${score.playerName} a marqué ${score.points} points au quart ${score.quarter} à ${formattedTime}`,
            date: new Date(score.time), 
          });
        });

        // Ajouter les fautes dans le tableau des actions avec leur date
        data.fouls.forEach((foul: Foul) => {
          const formattedTime = this.formatDate(foul.time); // Formater l'heure de la faute
          this.actions.push({
            actionType: 'foul', // Type de l'action
            text: `${foul.playerName} a commis une faute de type ${foul.foulType} au quart ${foul.quarter} à ${formattedTime}`,
            date: new Date(foul.time), // Date de la faute
          });
        });

        // Trier les actions par la date de manière décroissante (du plus récent au plus ancien)
        this.actions.sort((a, b) => b.date.getTime() - a.date.getTime());

        // Mise à jour des scores des équipes
        this.homeTeamScore = data.homeTeamScore;
        this.awayTeamScore = data.awayTeamScore;

        console.log('Faits du match récupérés:', data);
      },
      (error) => {
        console.error('Erreur lors de la récupération des faits du match', error);
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
    let player, points, playerName;
    if (team === 'team1') {
      player = this.selectedPlayerTeam1;
      points = this.selectedPointsTeam1;
    } else if (team === 'team2') {
      player = this.selectedPlayerTeam2;
      points = this.selectedPointsTeam2;
    }

    // Vérification que le joueur existe et que les points sont définis
    if (player && points && this.matchId !== null) {
      // S'assurer que playerName est une chaîne de caractères valide
      playerName = player ? `${player.firstName} ${player.lastName}` : "Nom Inconnu";

      // S'assurer que playerName n'est pas undefined ou null
      playerName = playerName || "Nom Inconnu";

      // Créer l'objet scoreDto
      const scoreDto = {
        playerId: player.playerId,
        points: points,
        playerName: playerName // Assurez-vous que playerName est toujours une chaîne
      };
      // Enregistrer le score en appelant le service
      this.matchService.recordScore(this.matchId!, scoreDto).subscribe(
        (response) => {
          // Vérifier que la réponse est correcte et formatée
          console.log('Panier marqué avec succès', response);
          this.actions.push(`${player.firstName} ${player.lastName} a marqué un panier de ${points} points.`);
        },
        (error) => {
          console.error('Erreur lors de l\'enregistrement du panier', error);
        }
      );
    } else {
      console.error('Informations manquantes pour enregistrer le panier.');
    }
  }


  // Enregistrer une faute
  recordFoul(team: string): void {
    let player, foulType, playerName;

    // Sélectionner le joueur et le type de faute selon l'équipe choisie
    if (team === 'team1') {
      player = this.selectedPlayerTeam1;
      foulType = this.selectedFoulTypeTeam1;
    } else if (team === 'team2') {
      player = this.selectedPlayerTeam2;
      foulType = this.selectedFoulTypeTeam2;
    }

    // Vérifier que le joueur et le type de faute sont définis avant d'enregistrer
    if (player && foulType && this.matchId !== null) {
      // Assurer que playerName est bien une chaîne valide
      playerName = player ? `${player.firstName} ${player.lastName}` : "Nom Inconnu";

      // S'assurer que playerName n'est pas undefined ou null
      playerName = playerName || "Nom Inconnu";

      // Créer l'objet foulDto avec les données nécessaires
      const foulDto = {
        playerId: player.playerId,
        foulType: foulType,
        playerName: playerName
      };  // Utiliser l'heure correcte ou la calculer dynamiquement si nécessaire

      // Enregistrer la faute via le service
      this.matchService.recordFoul(this.matchId!, foulDto).subscribe(
        (response) => {
          // Ajouter la faute au tableau des actions avec un format approprié
          this.actions.push(`${player.firstName} ${player.lastName} a commis une faute de type ${foulType} au quart ${this.currentQuarter}`);
          console.log('Faute enregistrée avec succès', response);
        },
        (error) => {
          console.error('Erreur lors de l\'enregistrement de la faute', error);
        }
      );
    } else {
      console.error('Informations manquantes pour enregistrer la faute.');
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

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString('fr-FR', {
      weekday: 'short',  // Jour de la semaine (abrégé)
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  }

  // Fonction pour extraire et retourner un objet Date à partir d'une chaîne
  extractDate(action: string): Date {
    const timeStr = action.split('à ')[1];  // Extraire la partie de la date/heure après "à"
    return new Date(timeStr);  // Retourner l'objet Date pour le tri
  }
}
