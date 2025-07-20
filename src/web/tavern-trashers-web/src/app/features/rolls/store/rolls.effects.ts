import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import * as RollsActions from './rolls.actions';
import { RollsService } from '../services/rolls.service';

@Injectable()
export class RollsEffects {
  private actions$: Actions = inject(Actions);
  constructor(private rollsService: RollsService) {}

  readonly loadRolls$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RollsActions.loadRolls),
      mergeMap(() =>
        this.rollsService.getAllRolls().pipe(
          map((rolls) => RollsActions.loadRollsSuccess({ rolls })),
          catchError((error) => of(RollsActions.loadRollsFailure({ error })))
        )
      )
    )
  );
}
