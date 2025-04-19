import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  RouterStateSnapshot,
} from '@angular/router';
import { AuthFacade } from './auth.facade';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  router: RouterStateSnapshot
) => {
  const auth = inject(AuthFacade);

  return auth.isLoggedIn$.pipe(
    map((isLoggedIn) => {
      if (isLoggedIn) {
        return true;
      } else {
        const redirectUrl = window.location.origin + router.url;
        console.log('Redirecting to login with redirect URL:', redirectUrl);
        auth.login(redirectUrl);
        return false;
      }
    })
  );
};
