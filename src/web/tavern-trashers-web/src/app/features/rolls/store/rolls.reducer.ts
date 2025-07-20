import { createReducer, on } from '@ngrx/store';
import * as RollsActions from './rolls.actions';
import { Roll } from '../models/roll.model';

export interface RollsState {
  rolls: Roll[];
  loading: boolean;
  error: any;
}

export const initialState: RollsState = {
  rolls: [],
  loading: false,
  error: null,
};

export const rollsReducer = createReducer(
  initialState,
  on(RollsActions.loadRolls, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(RollsActions.loadRollsSuccess, (state, { rolls }) => ({
    ...state,
    rolls,
    loading: false,
  })),
  on(RollsActions.loadRollsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  }))
);
