import { createReducer, on } from '@ngrx/store';
import { setBreakpoint } from './layout.actions';

export interface LayoutState {
  breakpoint: 'mobile' | 'desktop';
}

export const initialState: LayoutState = {
  breakpoint: 'mobile',
};

export const layoutReducer = createReducer(
  initialState,
  on(setBreakpoint, (state, { breakpoint }) => ({ ...state, breakpoint }))
);
