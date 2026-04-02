import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { OAuthService } from 'angular-oauth2-oidc';
import { EMPTY, catchError, map, of, switchMap, withLatestFrom } from 'rxjs';
import { HubService } from '../../../common/hubs/hub.service';
import { ResourceChangedNotification } from '../models/resource-changed-notification.model';
import { loginSuccess } from '../../auth/store/auth.actions';
import {
  clearCampaignSelection,
  selectCampaign,
} from '../../campaigns/store/campaigns.actions';
import { selectSelectedCampaignId } from '../../campaigns/store/campaigns.selectors';
import {
  connectHub,
  connectHubFailure,
  connectHubSuccess,
  hubResourceChanged,
  joinCampaignGroup,
  leaveCampaignGroup,
} from './hub.actions';
import { loadCharacters } from '../../characters/store/characters.actions';

@Injectable()
export class HubEffects {
  private actions$ = inject(Actions);

  constructor(
    private hubService: HubService,
    private oauthService: OAuthService,
    private store: Store
  ) {}

  connectOnLogin$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loginSuccess),
      map(() => connectHub())
    )
  );

  connect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(connectHub),
      switchMap(() =>
        this.hubService
          .connect(() => this.oauthService.getAccessToken())
          .pipe(
            map(() => connectHubSuccess()),
            catchError((error) => of(connectHubFailure({ error })))
          )
      )
    )
  );

  listenForResourceChanges$ = createEffect(() =>
    this.actions$.pipe(
      ofType(connectHubSuccess),
      switchMap(() =>
        this.hubService
          .on<ResourceChangedNotification>('ResourceChanged')
          .pipe(map((notification) => hubResourceChanged({ notification })))
      )
    )
  );

  joinCampaignGroupOnSelect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(selectCampaign),
      map(({ campaignId }) => joinCampaignGroup({ campaignId }))
    )
  );

  joinCampaignGroup$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(joinCampaignGroup),
        switchMap(({ campaignId }) =>
          this.hubService.joinGroup(`campaign:${campaignId}`).pipe(
            catchError(() => EMPTY)
          )
        )
      ),
    { dispatch: false }
  );

  leaveCampaignGroupOnClear$ = createEffect(() =>
    this.actions$.pipe(
      ofType(clearCampaignSelection),
      withLatestFrom(this.store.select(selectSelectedCampaignId)),
      map(([, campaignId]) => leaveCampaignGroup({ campaignId: campaignId ?? '' }))
    )
  );

  leaveCampaignGroup$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(leaveCampaignGroup),
        switchMap(({ campaignId }) => {
          if (!campaignId) return EMPTY;
          return this.hubService.leaveGroup(`campaign:${campaignId}`).pipe(
            catchError(() => EMPTY)
          );
        })
      ),
    { dispatch: false }
  );

  reloadCharactersOnResourceChange$ = createEffect(() =>
    this.actions$.pipe(
      ofType(hubResourceChanged),
      map(({ notification }) =>
        loadCharacters({ campaignId: notification.campaignId })
      )
    )
  );
}
