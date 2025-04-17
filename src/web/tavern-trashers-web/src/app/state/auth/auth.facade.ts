import { inject, Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { login, logout, register, tryAutoLogin } from './auth.actions';
import { Observable } from 'rxjs';
import { AuthState } from './auth.reducer';
import { selectAuth } from './auth.selectors';
import { UserRegistrationRequest } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthFacade {
  constructor(private store: Store) {}

  get state$(): Observable<AuthState> {
    return this.store.select(selectAuth);
  }

  register(userRegistrationRequest: UserRegistrationRequest): void {
    this.store.dispatch(register({ userRegistrationRequest }));
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
