import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { MatchListComponent } from './pages/match-list/match-list.component';
import { AddMatchComponent } from './pages/add-match/add-match.component';  // Import de FormsModule

import { MatchService } from './services/match.service';
import { FormsModule } from '@angular/forms';
import { EncodingFactsComponent } from './pages/encoding-facts/encoding-facts.component';
import { MatchSummaryComponent } from './pages/match-summary/match-summary.component';



@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    MatchListComponent,
    AddMatchComponent,
    EncodingFactsComponent,
    MatchSummaryComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule

  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
