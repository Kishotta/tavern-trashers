import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of } from 'rxjs';
import { CharactersService } from '../services/characters.service';
import {
  createCharacter,
  createCharacterFailure,
  createCharacterSuccess,
  loadCharacters,
  loadCharactersFailure,
  loadCharactersSuccess,
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
}
