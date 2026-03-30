import { createSelector } from '@ngrx/store';

export const selectCharactersState = (state: any) => state.characters;

export const selectCharacters = createSelector(
  selectCharactersState,
  (state) => state.characters
);

export const selectCharactersLoading = createSelector(
  selectCharactersState,
  (state) => state.loading
);

export const selectCharactersCreating = createSelector(
  selectCharactersState,
  (state) => state.creating
);
