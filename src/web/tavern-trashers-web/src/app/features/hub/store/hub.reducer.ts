import { createReducer, on } from '@ngrx/store';
import { ResourceChangedNotification } from '../models/resource-changed-notification.model';
import {
  connectHub,
  connectHubFailure,
  connectHubSuccess,
  hubResourceChanged,
} from './hub.actions';

export interface HubNotification extends ResourceChangedNotification {
  timestamp: string;
}

export interface HubState {
  connected: boolean;
  notifications: HubNotification[];
}

export const initialHubState: HubState = {
  connected: false,
  notifications: [],
};

const MAX_NOTIFICATIONS = 50;

export const hubReducer = createReducer(
  initialHubState,
  on(connectHub, (state) => ({ ...state, connected: false })),
  on(connectHubSuccess, (state) => ({ ...state, connected: true })),
  on(connectHubFailure, (state) => ({ ...state, connected: false })),
  on(hubResourceChanged, (state, { notification }) => ({
    ...state,
    notifications: [
      { ...notification, timestamp: new Date().toISOString() },
      ...state.notifications,
    ].slice(0, MAX_NOTIFICATIONS),
  }))
);
