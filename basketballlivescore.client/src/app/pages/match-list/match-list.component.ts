import { Component, OnInit } from '@angular/core';
import { MatchService } from '../../services/match.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-match-list',
  templateUrl: './match-list.component.html',
  styleUrls: ['./match-list.component.css']
})
export class MatchListComponent implements OnInit {
  matches: any[] = [];
  loading: boolean = true;

  constructor(private matchService: MatchService, private router: Router) { }

  ngOnInit(): void {
    this.loadMatches();
  }

  loadMatches(): void {
    this.matchService.getMatches().subscribe(
      (data) => {
        console.log(data); // Affiche les données pour vérifier leur structure
        this.matches = data;
        this.loading = false;
      },
      (error) => {
        console.error('Error fetching matches', error);
        this.loading = false;
      }
    );
  }


  // La méthode 'addNewMatch' pour rediriger vers la page d'ajout de match
  addNewMatch(): void {
    this.router.navigate(['/add-match']);  // Redirige vers la page d'ajout de match
  }

  encodingFacts(matchId: number): void {
    if (matchId) {
      this.router.navigate([matchId, 'encoding-facts']);  // Format correct pour inclure l'ID dans l'URL
    } else {
      console.error('Match ID is undefined');  // Vérifier si l'ID du match est correct
    }
  }

}
