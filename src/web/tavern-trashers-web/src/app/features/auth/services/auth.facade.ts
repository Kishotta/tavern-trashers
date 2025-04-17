import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AuthState } from '../store/auth.reducer';
import { selectAuth } from '../store/auth.selectors';
import { UserRegistrationRequest } from './user-registration-request';
import { register, tryAutoLogin, login, logout } from '../store/auth.actions';

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
