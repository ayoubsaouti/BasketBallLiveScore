import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { MatchListComponent } from './pages/match-list/match-list.component';
import { AddMatchComponent } from './pages/add-match/add-match.component';
import { EncodingFactsComponent } from './pages/encoding-facts/encoding-facts.component';
import { MatchSummaryComponent } from './pages/match-summary/match-summary.component';
import { AuthGuard } from './services/guards/auth.guard';  // Import du guard
import { RoleGuard } from './services/guards/role.guard';  // Import du role guard

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: MatchListComponent },

  // Protège les routes par AuthGuard + RoleGuard pour certains rôles
  { path: 'add-match', component: AddMatchComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } },
  { path: ':id/encoding-facts', component: EncodingFactsComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } },

  { path: ':id/match-summary', component: MatchSummaryComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
