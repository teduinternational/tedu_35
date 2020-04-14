import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services';
import { SystemConstants } from '../constants';

@Injectable()
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private authService: AuthService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authService.isAuthenticated()) {
      const functionCode = route.data['functionCode'] as string;
      const permissions = JSON.parse(this.authService.profile.permissions);
      if (permissions && permissions.filter(x => x === functionCode + '_' + SystemConstants.VIEW_ACTION).length > 0) {
        return true;
      } else {
        this.router.navigate(['/access-denied'], {
          queryParams: { redirect: state.url }
        });
        return false;
      }
    }
    this.router.navigate(['/login'], { queryParams: { redirect: state.url }, replaceUrl: true });
    return false;
  }

}