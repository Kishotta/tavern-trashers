import { createAction, props } from '@ngrx/store';

export const setBreakpoint = createAction(
  '[Layout] Set Breakpoint',
  props<{ breakpoint: 'mobile' | 'desktop' }>()
);
