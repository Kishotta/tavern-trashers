import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap, tap } from 'rxjs';
import {
  login,
  logout,
  register,
  registerFailure,
  registerSuccess,
  tryAutoLogin,
} from './auth.actions';
import { AuthService } from '../services/auth.service';
import { ProblemDetails } from '../../../state/problemDetails';

@Injectable()
export class AuthEffects {
  private actions$: Actions = inject(Actions);
  constructor(private authService: AuthService) {}

  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(register),
      switchMap(({ userRegistrationRequest }) =>
        this.authService.register(userRegistrationRequest).pipe(
          map((authToken) => {
            return registerSuccess({ authToken });
          }),
          catchError((error: ProblemDetails) => {
            return of(registerFailure({ error: error.title }));
          })
        )
      )
    )
  );

  registerSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(registerSuccess),
      tap(({ authToken }) => {
        this.authService.forceLogin(
          authToken.access_token,
          authToken.id_token,
          authToken.refresh_token
        );
      }),
      map(() => tryAutoLogin())
    )
  );

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
