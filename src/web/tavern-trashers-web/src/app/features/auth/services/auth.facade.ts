import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AuthState } from '../store/auth.reducer';
import { selectAuth, selectIsLoggedIn } from '../store/auth.selectors';
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

  get isLoggedIn$(): Observable<boolean> {
    return this.store.select(selectIsLoggedIn);
  }

  register(userRegistrationRequest: UserRegistrationRequest): void {
    this.store.dispatch(register({ userRegistrationRequest }));
  }

  tryAutoLogin(): void {
    this.store.dispatch(tryAutoLogin());
  }

  login(redirectUrl: string | null = null): void {
    console.log('Dispatching login action with redirect URL:', redirectUrl);
    this.store.dispatch(login({ redirectUrl }));
  }

  logout(): void {
    this.store.dispatch(logout());
  }
}
