import { createSelector } from '@ngrx/store';

export const selectLayoutState = (state: any) => state.layout;

export const selectBreakpoint = createSelector(
  selectLayoutState,
  (state) => state.breakpoint
);
