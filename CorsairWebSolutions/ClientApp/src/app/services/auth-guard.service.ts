import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service';


@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(private auth: AuthService, private router: Router) { }

  canActivate(route: import("@angular/router").ActivatedRouteSnapshot, state: import("@angular/router").RouterStateSnapshot): boolean | import("rxjs").Observable<boolean> | Promise<boolean> {

    return this.auth.isLoggedInObs().toPromise()
      .then(s => {
        if (s == true)
          return true;
        else {
          this.auth.startSigninMainWindow();
          return false;
        }

      });
  }
}
