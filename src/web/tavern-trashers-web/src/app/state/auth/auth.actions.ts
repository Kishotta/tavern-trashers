import { createAction, props } from '@ngrx/store';

export const tryAutoLogin = createAction('[Auth] Try Auto Login');

export const login = createAction('[Auth] Login');

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ user: any }>()
);

export const logout = createAction('[Auth] Logout');

export const tokenRefreshed = createAction(
  '[Auth] Token Refreshed',
  props<{ accessToken: string }>()
);
