import { createFeatureSelector, createSelector } from '@ngrx/store';
import { HubState } from './hub.reducer';

export const selectHubState = createFeatureSelector<HubState>('hub');

export const selectHubConnected = createSelector(
  selectHubState,
  (state) => state.connected
);

export const selectHubNotifications = createSelector(
  selectHubState,
  (state) => state.notifications
);
