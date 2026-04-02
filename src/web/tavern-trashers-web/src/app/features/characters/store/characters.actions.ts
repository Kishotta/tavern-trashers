import { createAction, props } from '@ngrx/store';
import { Character, ResetTrigger } from '../models/character.model';

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

export const bulkRestoreResources = createAction(
  '[Characters] Bulk Restore Resources',
  props<{ campaignId: string; trigger: ResetTrigger }>()
);

export const bulkRestoreResourcesSuccess = createAction(
  '[Characters] Bulk Restore Resources Success'
);

export const bulkRestoreResourcesFailure = createAction(
  '[Characters] Bulk Restore Resources Failure',
  props<{ error: any }>()
);

export const useResource = createAction(
  '[Characters] Use Resource',
  props<{ characterId: string; resourceId: string }>()
);

export const applyResource = createAction(
  '[Characters] Apply Resource',
  props<{ characterId: string; resourceId: string }>()
);

export const restoreResource = createAction(
  '[Characters] Restore Resource',
  props<{ characterId: string; resourceId: string }>()
);
