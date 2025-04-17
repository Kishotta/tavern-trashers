import { Routes } from '@angular/router';

export const campaignRoutes: Routes = [
  {
    path: ':id',
    loadComponent: () =>
      import('./components/campaign-detail/campaign-detail.component').then(
        (m) => m.CampaignDetailComponent
      ),
  },
];

