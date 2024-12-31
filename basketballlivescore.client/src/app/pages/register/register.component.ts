// src/app/pages/register/register.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) { }

  // Fonction pour enregistrer un utilisateur
  register() {
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Les mots de passe ne correspondent pas.';
      return;
    }

    const registerData = {
      email: this.email,
      password: this.password,
    };

    this.authService.register(registerData).subscribe(
      (response) => {
        // Une fois l'utilisateur enregistré, rediriger vers la page de connexion
        this.router.navigate(['/login']);
      },
      (error) => {
        this.errorMessage = 'Erreur lors de l\'enregistrement. Veuillez réessayer.';
      }
    );
  }
}
