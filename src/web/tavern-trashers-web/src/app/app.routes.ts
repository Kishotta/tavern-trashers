import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CampaignsComponent } from './campaigns/campaigns.component';
import { CampaignDetailComponent } from './campaigns/campaign-detail/campaign-detail.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  {
    path: 'campaigns',
    component: CampaignsComponent,
    children: [
      { path: ':id', component: CampaignDetailComponent },
    ],
  },
];
