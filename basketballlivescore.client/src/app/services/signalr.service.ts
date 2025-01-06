import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  private hubConnection: HubConnection | null = null;
  private matchUpdateSource = new BehaviorSubject<any>(null); // Contient les mises à jour des événements
  private scoreUpdateSource = new BehaviorSubject<any>(null); // Contient les mises à jour des scores
  private timerUpdateSource = new BehaviorSubject<number>(0);
  private matchFinishUpdateSource = new BehaviorSubject<any>(null);
  private quarterUpdateSource = new BehaviorSubject<any>(null);

  matchUpdate$ = this.matchUpdateSource.asObservable();
  scoreUpdate$ = this.scoreUpdateSource.asObservable();
  timerUpdate$ = this.timerUpdateSource.asObservable();
  matchFinishUpdateSource$ = this.matchUpdateSource.asObservable();
  quarterUpdate$ = this.quarterUpdateSource.asObservable();

  constructor() {
    this.createHubConnection(); // Crée la connexion SignalR dès le démarrage
  }

  // Créer la connexion SignalR
  createHubConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7295/Hub/matchHub')  // Assurez-vous que l'URL est correcte
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected successfully'))
      .catch((err) => console.error('Error while starting connection: ' + err));

    // Écouter les mises à jour en temps réel des événements (score, faute)
    this.hubConnection?.on('ReceiveUpdate', (matchId, actionType, message, elapsedTime) => {
      this.matchUpdateSource.next({ matchId, actionType, message, elapsedTime });
    });

    // Écouter les mises à jour des scores des équipes
    this.hubConnection?.on('ReceiveScoreUpdate', (matchId, homeTeamScore, awayTeamScore) => {
      this.scoreUpdateSource.next({ matchId, homeTeamScore, awayTeamScore });
    });

    // Écouter les mises à jour du timer
    this.hubConnection?.on('ReceiveTimerUpdate', (matchId, elapsedTime) => {
      this.timerUpdateSource.next(elapsedTime);  // Met à jour le timer avec la nouvelle valeur
    });

    // Écouter l'événement pour la fin du match
    this.hubConnection?.on('ReceiveMatchFinished', (matchId, isFinished) => {
      this.matchFinishUpdateSource.next({ matchId, isFinished });
    });

    this.hubConnection?.on('ReceiveQuarterUpdate', (matchId, currentQuarter) => {
      this.quarterUpdateSource.next({ matchId, currentQuarter });
    });
  }

  stopConnection(): void {
    this.hubConnection?.stop().catch((err) => console.error('Error while stopping connection: ' + err));
  }
}
