import { createReducer, on } from '@ngrx/store';
import { ProblemDetails } from '../../../state/problemDetails';
import {
  clearCampaignSelection,
  loadCampaigns,
  loadCampaignsFailure,
  loadCampaignsSuccess,
  selectCampaign,
} from './campaigns.actions';

export interface Campaign {
  id: string;
  title: string;
  description: string;
}

export interface CampaignsState {
  campaigns: Campaign[];
  loading: boolean;
  error: ProblemDetails | null;
  selectedCampaignId: string | null;
}

export const initialState: CampaignsState = {
  campaigns: [],
  loading: false,
  error: null,
  selectedCampaignId: null,
};

export const campaignsReducer = createReducer(
  initialState,
  on(loadCampaigns, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(loadCampaignsSuccess, (state, { campaigns }) => ({
    ...state,
    loading: false,
    campaigns,
    error: null,
  })),
  on(loadCampaignsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),
  on(selectCampaign, (state, { campaignId }) => ({
    ...state,
    selectedCampaignId: campaignId,
  })),
  on(clearCampaignSelection, (state) => ({
    ...state,
    selectedCampaignId: null,
  }))
);
