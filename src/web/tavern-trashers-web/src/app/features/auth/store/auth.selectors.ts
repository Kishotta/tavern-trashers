import { createSelector } from '@ngrx/store';

export const selectAuth = (state: any) => state.auth;

export const selectIsLoggedIn = createSelector(
  selectAuth,
  (auth) => !!auth.user
);
