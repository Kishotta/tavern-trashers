import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { CampaignsService } from '../../services/campaigns.service';
import {
  loadCampaigns,
  loadCampaignsFailure,
  loadCampaignsSuccess,
} from './campaigns.actions';
import { catchError, map, mergeMap, of } from 'rxjs';

@Injectable()
export class CampaignsEffects {
  private actions$: Actions = inject(Actions);
  constructor(private campaignsService: CampaignsService) {}

  loadResources$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadCampaigns),
      mergeMap(() =>
        this.campaignsService.getCampaigns().pipe(
          map((campaigns) => loadCampaignsSuccess({ campaigns })),
          catchError((error) =>
            of(
              loadCampaignsFailure({
                error: error || 'Failed to load campaigns',
              })
            )
          )
        )
      )
    )
  );
}
