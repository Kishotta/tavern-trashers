import { createReducer, on } from '@ngrx/store';
import { loginSuccess, logout, tokenRefreshed } from './auth.actions';

export interface AuthState {
  user: any | null;
  accessToken: string | null;
}

export const initialState: AuthState = {
  user: null,
  accessToken: null,
};

export const authReducer = createReducer(
  initialState,
  on(loginSuccess, (state, { user }) => ({ ...state, user })),
  on(tokenRefreshed, (state, { accessToken }) => ({
    ...state,
    accessToken,
  })),
  on(logout, () => initialState)
);
