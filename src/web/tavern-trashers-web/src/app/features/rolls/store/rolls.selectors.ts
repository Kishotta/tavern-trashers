import { createFeatureSelector, createSelector } from '@ngrx/store';
import { RollsState } from './rolls.reducer';

export const selectRollsState = createFeatureSelector<RollsState>('rolls');

export const selectAllRolls = createSelector(
  selectRollsState,
  (state) => state.rolls
);

export const selectRollsLoading = createSelector(
  selectRollsState,
  (state) => state.loading
);

export const selectRollsError = createSelector(
  selectRollsState,
  (state) => state.error
);
