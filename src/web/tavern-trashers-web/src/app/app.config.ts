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
import { AuthEffects } from './features/auth/store/auth.effects';
import { provideEffects } from '@ngrx/effects';
import { authReducer } from './features/auth/store/auth.reducer';
import { campaignsReducer } from './features/campaigns/store/campaigns.reducer';
import { CampaignsEffects } from './features/campaigns/store/campaigns.effects';
import { layoutReducer } from './state/layout/layout.reducer';
import { rollsReducer } from './features/rolls/store/rolls.reducer';
import { RollsEffects } from './features/rolls/store/rolls.effects';
import { provideAnimations } from '@angular/platform-browser/animations';

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
    provideState({ name: 'rolls', reducer: rollsReducer }),
    provideEffects([AuthEffects, CampaignsEffects, RollsEffects]),
    provideStoreDevtools({
      maxAge: 25, // Retains last 25 states
      logOnly: !isDevMode(), // Restrict extension to log-only mode
      autoPause: true, // Pauses recording actions and state changes when the extension window is not open
      trace: true, //  If set to true, will include stack trace for every dispatched action, so you can see it in trace tab jumping directly to that part of code
      traceLimit: 75, // maximum stack trace frames to be stored (in case trace option was provided as true)
      connectInZone: true, // If set to true, the connection is established within the Angular zone
    }),
    provideAnimations(),
  ],
};
