import { createAction, props } from '@ngrx/store';
import { AuthToken } from '../services/auth-token';
import { UserRegistrationRequest } from '../services/user-registration-request';

export const register = createAction(
  '[Auth] Register',
  props<{ userRegistrationRequest: UserRegistrationRequest }>()
);

export const registerSuccess = createAction(
  '[Auth] Register Success',
  props<{ authToken: AuthToken }>()
);

export const registerFailure = createAction(
  '[Auth] Register Failure',
  props<{ error: string }>()
);

export const tryAutoLogin = createAction('[Auth] Try Auto Login');

export const login = createAction(
  '[Auth] Login',
  props<{ redirectUrl: string | null }>()
);

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ user: any }>()
);

export const logout = createAction('[Auth] Logout');
