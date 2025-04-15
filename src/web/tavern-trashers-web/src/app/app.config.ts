import {
  ApplicationConfig,
  isDevMode,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter, withRouterConfig } from '@angular/router';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { routes } from './app.routes';
import {
  provideHttpClient,
  withInterceptors,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { provideState, provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { AuthEffects } from './state/auth/auth.effects';
import { provideEffects } from '@ngrx/effects';
import { authReducer } from './state/auth/auth.reducer';
import { campaignsReducer } from './state/campaigns/campaigns.reducer';
import { CampaignsEffects } from './state/campaigns/campaigns.effects';
import { layoutReducer } from './state/layout/layout.reducer';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withRouterConfig({ onSameUrlNavigation: 'reload' })),
    provideHttpClient(withInterceptors([]), withInterceptorsFromDi()),
    provideOAuthClient({
      resourceServer: {
        allowedUrls: ['/api'],
        sendAccessToken: true,
      },
    }),
    provideStore(),
    provideState({ name: 'auth', reducer: authReducer }),
    provideState({ name: 'campaigns', reducer: campaignsReducer }),
    provideState({ name: 'layout', reducer: layoutReducer }),
    provideEffects([AuthEffects, CampaignsEffects]),
    provideStoreDevtools({
      maxAge: 25, // Retains last 25 states
      logOnly: !isDevMode(), // Restrict extension to log-only mode
      autoPause: true, // Pauses recording actions and state changes when the extension window is not open
      trace: true, //  If set to true, will include stack trace for every dispatched action, so you can see it in trace tab jumping directly to that part of code
      traceLimit: 75, // maximum stack trace frames to be stored (in case trace option was provided as true)
      connectInZone: true, // If set to true, the connection is established within the Angular zone
    }),
  ],
};
