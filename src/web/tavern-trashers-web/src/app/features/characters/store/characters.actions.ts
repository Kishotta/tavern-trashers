import { createAction, props } from '@ngrx/store';
import { Character } from '../models/character.model';

export const loadCharacters = createAction(
  '[Characters] Load Characters',
  props<{ campaignId: string }>()
);

export const loadCharactersSuccess = createAction(
  '[Characters] Load Characters Success',
  props<{ characters: Character[] }>()
);

export const loadCharactersFailure = createAction(
  '[Characters] Load Characters Failure',
  props<{ error: any }>()
);

export const createCharacter = createAction(
  '[Characters] Create Character',
  props<{ name: string; level: number; campaignId: string }>()
);

export const createCharacterSuccess = createAction(
  '[Characters] Create Character Success',
  props<{ character: Character }>()
);

export const createCharacterFailure = createAction(
  '[Characters] Create Character Failure',
  props<{ error: any }>()
);
