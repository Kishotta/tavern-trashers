import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import(
        './features/dashboard/components/dashboard-page/dashboard-page.component'
      ).then((m) => m.DashboardPageComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import(
        './features/auth/components/user-registration-page/user-registration-page.component'
      ).then((m) => m.UserRegistrationPageComponent),
  },
  {
    path: 'campaigns',
    loadComponent: () =>
      import(
        './features/campaigns/components/campaigns-page/campaigns-page.component'
      ).then((m) => m.CampaignsPageComponent),
    loadChildren: () =>
      import('./features/campaigns/campaigns.routes').then(
        (m) => m.campaignRoutes
      ),
  },
];
