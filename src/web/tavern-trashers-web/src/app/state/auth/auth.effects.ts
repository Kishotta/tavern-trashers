import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { tap } from 'rxjs';
import { login, logout, tryAutoLogin } from './auth.actions';
import { AuthService } from './auth.service';

@Injectable()
export class AuthEffects {
  private actions$: Actions = inject(Actions);
  constructor(private authService: AuthService) {}

  tryAutoLogin$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(tryAutoLogin),
        tap(() => {
          if (!this.authService.isLoggedIn()) {
            this.authService.tryLogIn();
          }
        })
      ),
    { dispatch: false }
  );

  login$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(login),
        tap(() => {
          this.authService.login();
        })
      ),
    { dispatch: false }
  );

  logout$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(logout),
        tap(() => {
          this.authService.logout();
        })
      ),
    { dispatch: false }
  );
}
