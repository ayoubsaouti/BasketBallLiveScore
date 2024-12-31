// src/app/pages/add-match/add-match.component.ts
import { Component } from '@angular/core';
import { MatchService } from '../../services/match.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-match',
  templateUrl: './add-match.component.html',
  styleUrls: ['./add-match.component.css']
})
export class AddMatchComponent {

  matchData = {
    matchNumber: '',
    competition: '',
    matchDate: '',
    periods: 4,
    periodDuration: 10,
    overtimeDuration: 5
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

  constructor(private matchService: MatchService, private router: Router) { }

  createMatch(): void {
    // Créer le match
    this.matchService.createMatch(this.matchData).subscribe((match) => {
      // Ajouter les équipes
      this.matchService.addTeamsToMatch(match.matchId, this.teamsData).subscribe(() => {
        // Ajouter les joueurs (en supposant que vous avez des joueurs à ajouter)
        this.matchService.addPlayersToMatch(match.matchId, {
          homePlayers: [],  // Ajoutez ici les joueurs à domicile
          awayPlayers: []   // Ajoutez ici les joueurs visiteurs
        }).subscribe(() => {
          alert('Match ajouté avec succès !');
          this.router.navigate(['/match-list']);  // Rediriger vers la page de liste des matchs
        });
      });
    });
  }
}
