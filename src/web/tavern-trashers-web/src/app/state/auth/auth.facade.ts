import { inject, Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { login, logout, tryAutoLogin } from './auth.actions';
import { Observable } from 'rxjs';
import { AuthState } from './auth.reducer';
import { selectAuth } from './auth.selectors';

@Injectable({
  providedIn: 'root',
})
export class AuthFacade {
  constructor(private store: Store) {}

  get state$(): Observable<AuthState> {
    return this.store.select(selectAuth);
  }

  tryAutoLogin(): void {
    this.store.dispatch(tryAutoLogin());
  }

  login(): void {
    this.store.dispatch(login());
  }

  logout(): void {
    this.store.dispatch(logout());
  }
}
