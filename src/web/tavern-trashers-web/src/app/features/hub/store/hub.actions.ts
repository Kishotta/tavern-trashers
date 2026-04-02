import { createAction, props } from '@ngrx/store';
import { ResourceChangedNotification } from '../models/resource-changed-notification.model';

export const connectHub = createAction('[Hub] Connect');

export const connectHubSuccess = createAction('[Hub] Connect Success');

export const connectHubFailure = createAction(
  '[Hub] Connect Failure',
  props<{ error: any }>()
);

export const joinCampaignGroup = createAction(
  '[Hub] Join Campaign Group',
  props<{ campaignId: string }>()
);

export const leaveCampaignGroup = createAction(
  '[Hub] Leave Campaign Group',
  props<{ campaignId: string }>()
);

export const hubResourceChanged = createAction(
  '[Hub] Resource Changed',
  props<{ notification: ResourceChangedNotification }>()
);
