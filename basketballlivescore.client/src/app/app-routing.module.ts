import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { MatchListComponent } from './pages/match-list/match-list.component';
import { AddMatchComponent } from './pages/add-match/add-match.component';


const routes: Routes = [
  { path: '', component: LoginComponent }, // default redirection
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: MatchListComponent },
  { path: 'add-match', component: AddMatchComponent },


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
