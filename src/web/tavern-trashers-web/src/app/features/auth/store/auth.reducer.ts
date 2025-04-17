import { createReducer, on } from '@ngrx/store';
import { loginSuccess, logout } from './auth.actions';

export interface User {
  id: string;
  name: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface AuthState {
  user: User | null;
}

export const initialState: AuthState = {
  user: null,
};

export const authReducer = createReducer(
  initialState,
  on(loginSuccess, (state, { user }) => ({ ...state, user })),
  on(logout, () => initialState)
);
