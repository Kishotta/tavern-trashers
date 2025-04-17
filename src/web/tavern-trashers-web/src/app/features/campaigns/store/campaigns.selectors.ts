import { createSelector } from '@ngrx/store';
import { Campaign } from './campaigns.reducer';

export const selectCampaignsState = (state: any) => state.campaigns;

export const selectCampaigns = createSelector(
  selectCampaignsState,
  (state) => state.campaigns
);

export const selectSelectedCampaignId = createSelector(
  selectCampaignsState,
  (state) => state.selectedCampaignId
);

export const selectSelectedCampaign = createSelector(
  selectCampaigns,
  selectSelectedCampaignId,
  (campaigns, selectedCampaignId) => {
    return campaigns.find(
      (campaign: Campaign) => campaign.id === selectedCampaignId
    );
  }
);
