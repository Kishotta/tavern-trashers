import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, EMPTY, map, mergeMap, of } from 'rxjs';
import { CharactersService } from '../services/characters.service';
import {
  applyResource,
  bulkRestoreResources,
  bulkRestoreResourcesFailure,
  bulkRestoreResourcesSuccess,
  createCharacter,
  createCharacterFailure,
  createCharacterSuccess,
  loadCharacters,
  loadCharactersFailure,
  loadCharactersSuccess,
  restoreResource,
  useResource,
} from './characters.actions';

@Injectable()
export class CharactersEffects {
  private actions$: Actions = inject(Actions);

  constructor(private charactersService: CharactersService) {}

  loadCharacters$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadCharacters),
      mergeMap(({ campaignId }) =>
        this.charactersService.getCharactersByCampaign(campaignId).pipe(
          map((characters) => loadCharactersSuccess({ characters })),
          catchError((error) => of(loadCharactersFailure({ error })))
        )
      )
    )
  );

  createCharacter$ = createEffect(() =>
    this.actions$.pipe(
      ofType(createCharacter),
      mergeMap(({ name, level, campaignId }) =>
        this.charactersService.createCharacter({ name, level, campaignId }).pipe(
          map((character) => createCharacterSuccess({ character })),
          catchError((error) => of(createCharacterFailure({ error })))
        )
      )
    )
  );

  bulkRestoreResources$ = createEffect(() =>
    this.actions$.pipe(
      ofType(bulkRestoreResources),
      mergeMap(({ campaignId, trigger }) =>
        this.charactersService.bulkRestoreResources(campaignId, trigger).pipe(
          map(() => bulkRestoreResourcesSuccess()),
          catchError((error) => of(bulkRestoreResourcesFailure({ error })))
        )
      )
    )
  );

  useResource$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(useResource),
        mergeMap(({ characterId, resourceId }) =>
          this.charactersService.useResource(characterId, resourceId).pipe(
            catchError((error) => {
              console.error('Failed to use resource', error);
              return EMPTY;
            })
          )
        )
      ),
    { dispatch: false }
  );

  applyResource$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(applyResource),
        mergeMap(({ characterId, resourceId }) =>
          this.charactersService.applyResource(characterId, resourceId).pipe(
            catchError((error) => {
              console.error('Failed to apply resource', error);
              return EMPTY;
            })
          )
        )
      ),
    { dispatch: false }
  );

  restoreResource$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(restoreResource),
        mergeMap(({ characterId, resourceId }) =>
          this.charactersService.restoreResource(characterId, resourceId).pipe(
            catchError((error) => {
              console.error('Failed to restore resource', error);
              return EMPTY;
            })
          )
        )
      ),
    { dispatch: false }
  );
}
