import { createReducer, on } from '@ngrx/store';
import { Character } from '../models/character.model';
import {
  bulkRestoreResources,
  bulkRestoreResourcesFailure,
  bulkRestoreResourcesSuccess,
  createCharacter,
  createCharacterFailure,
  createCharacterSuccess,
  loadCharacters,
  loadCharactersFailure,
  loadCharactersSuccess,
} from './characters.actions';

export interface CharactersState {
  characters: Character[];
  loading: boolean;
  creating: boolean;
  restoring: boolean;
  error: any;
}

export const initialState: CharactersState = {
  characters: [],
  loading: false,
  creating: false,
  restoring: false,
  error: null,
};

export const charactersReducer = createReducer(
  initialState,
  on(loadCharacters, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(loadCharactersSuccess, (state, { characters }) => ({
    ...state,
    loading: false,
    characters,
    error: null,
  })),
  on(loadCharactersFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),
  on(createCharacter, (state) => ({
    ...state,
    creating: true,
    error: null,
  })),
  on(createCharacterSuccess, (state, { character }) => ({
    ...state,
    creating: false,
    characters: [...state.characters, character],
  })),
  on(createCharacterFailure, (state, { error }) => ({
    ...state,
    creating: false,
    error,
  })),
  on(bulkRestoreResources, (state) => ({
    ...state,
    restoring: true,
    error: null,
  })),
  on(bulkRestoreResourcesSuccess, (state) => ({
    ...state,
    restoring: false,
  })),
  on(bulkRestoreResourcesFailure, (state, { error }) => ({
    ...state,
    restoring: false,
    error,
  }))
);
