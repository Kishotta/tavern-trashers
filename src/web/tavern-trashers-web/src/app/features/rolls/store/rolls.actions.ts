import { createAction, props } from '@ngrx/store';
import { Roll } from '../models/roll.model';

export const loadRolls = createAction('[Rolls] Load Rolls');
export const loadRollsSuccess = createAction(
  '[Rolls] Load Rolls Success',
  props<{ rolls: Roll[] }>()
);
export const loadRollsFailure = createAction(
  '[Rolls] Load Rolls Failure',
  props<{ error: any }>()
);
