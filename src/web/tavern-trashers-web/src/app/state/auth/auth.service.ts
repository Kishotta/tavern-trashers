import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { loginSuccess } from './auth.actions';
import { OAuthEvent, OAuthService, OAuthStorage } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from './auth.config';
import { filter, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private oauthService: OAuthService,
    private oauthStorage: OAuthStorage,
    private http: HttpClient,
    private store: Store
  ) {
    this.configureOAuthService();
    this.subscribeToOAuthEvents();
  }

  private configureOAuthService() {
    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.setupAutomaticSilentRefresh();
    this.oauthService.loadDiscoveryDocument();
  }

  private subscribeToOAuthEvents() {
    this.oauthService.events
      .pipe(
        filter(
          (event: OAuthEvent) =>
            event.type == 'discovery_document_loaded' ||
            event.type == 'token_received' ||
            event.type == 'token_refreshed'
        )
      )
      .subscribe(() => {
        const claims = this.oauthService.getIdentityClaims();
        if (!claims) {
          return;
        }
        this.store.dispatch(
          loginSuccess({
            user: {
              id: claims['sub'],
              name: claims['name'],
              firstName: claims['given_name'],
              lastName: claims['family_name'],
              email: claims['email'],
            },
          })
        );
      });
  }

  register(request: UserRegistrationRequest): Observable<AuthToken> {
    return this.http.post<AuthToken>('api/users/register', request);
  }

  forceLogin(accessToken: string, idToken: string, refreshToken: string): void {
    this.oauthStorage.setItem('access_token', accessToken);
    this.oauthStorage.setItem('id_token', idToken);
    this.oauthStorage.setItem('refresh_token', refreshToken);
  }

  tryLogIn(): void {
    this.oauthService.tryLogin();
  }

  login(): void {
    this.oauthService.initLoginFlow();
  }

  isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  logout(): void {
    this.oauthService.logOut();
  }
}

export interface UserRegistrationRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface AuthToken {
  access_token: string;
  token_type: string;
  refresh_token: string;
  expires_in: number;
  id_token: string;
}
