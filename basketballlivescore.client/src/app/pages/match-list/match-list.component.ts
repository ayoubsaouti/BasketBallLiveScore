import { Component, OnInit } from '@angular/core';
import { MatchService } from '../../services/match.service';
import { Router } from '@angular/router';  // Importez le Router pour la redirection

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
}
