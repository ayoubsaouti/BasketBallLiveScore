// src/app/pages/login/login.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) { }

  // Fonction de connexion
  login() {
    const loginData = {
      email: this.email,
      password: this.password,
    };

    this.authService.login(loginData).subscribe(
      (response) => {
        // Stocker le token JWT dans le localStorage pour l'utiliser dans les requêtes futures
        localStorage.setItem('token', response.token);
        this.router.navigate(['/home']);  // Rediriger après une connexion réussie
      },
      (error) => {
        this.errorMessage = 'Nom d\'utilisateur ou mot de passe incorrect';
      }
    );
  }
}
