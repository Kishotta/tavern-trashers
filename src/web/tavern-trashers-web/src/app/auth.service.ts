import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { loginSuccess, tokenRefreshed } from './state/auth/auth.actions';
import { OAuthEvent, OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from './auth.config';
import { filter } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private oauthService: OAuthService, private store: Store) {
    this.configureOAuthService();
    this.subscribeToOAuthEvents();
  }

  private configureOAuthService() {
    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.setupAutomaticSilentRefresh();
    this.oauthService.loadDiscoveryDocument();
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
        this.store.dispatch(loginSuccess({ user: claims }));
        const accessToken = this.oauthService.getAccessToken();
        this.store.dispatch(tokenRefreshed({ accessToken: accessToken }));
      });
  }
}
