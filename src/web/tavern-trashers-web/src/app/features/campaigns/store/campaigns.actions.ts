import { createAction, props } from '@ngrx/store';
import { ProblemDetails } from '../../../state/problemDetails';
import { Campaign } from './campaigns.reducer';

export const loadCampaigns = createAction('[Campaigns] Load Campaigns');

export const loadCampaignsSuccess = createAction(
  '[Campaigns] Load Campaigns Success',
  props<{ campaigns: Campaign[] }>()
);

export const loadCampaignsFailure = createAction(
  '[Campaigns] Load Campaigns Failure',
  props<{ error: ProblemDetails }>()
);

export const selectCampaign = createAction(
  '[Campaigns] Select Campaign',
  props<{ campaignId: string }>()
);

export const clearCampaignSelection = createAction(
  '[Campaigns] Clear Campaign Selection'
);
