import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../auth.service';  // Votre service d'authentification

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const userRole = this.authService.getRole();  // Assurez-vous d'avoir une méthode qui récupère le rôle de l'utilisateur

    // Utilisez l'indexeur pour accéder à la propriété expectedRole
    const role = next.data['role'];

    // Vérifiez si le rôle correspond
    if (userRole !== role) {
      // Redirection vers la page login si le rôle n'est pas autorisé
      this.router.navigate(['/login']);
      return false;
    }

    return true;
  }
}
