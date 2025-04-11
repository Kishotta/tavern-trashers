import { Routes } from '@angular/router';
import { CampaignsComponent } from './campaigns/campaigns.component';
import { CampaignListComponent } from './campaigns/campaign-list/campaign-list.component';
import { CampaignDetailComponent } from './campaigns/campaign-detail/campaign-detail.component';
import { DashboardComponent } from './dashboard/dashboard.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  {
    path: 'campaigns',
    component: CampaignsComponent,
    children: [
      { path: '', component: CampaignListComponent, outlet: 'list' },
      { path: ':id', component: CampaignDetailComponent, outlet: 'detail' },
    ],
  },
];
