import { Routes } from '@angular/router';
import { authGuard } from './features/auth/services/auth.guards';

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
    canActivate: [authGuard],
    loadComponent: () =>
      import(
        './features/campaigns/components/campaigns-page/campaigns-page.component'
      ).then((m) => m.CampaignsPageComponent),
    loadChildren: () =>
      import('./features/campaigns/campaigns.routes').then(
        (m) => m.campaignRoutes
      ),
  },
  {
    path: 'rolls',
    loadComponent: () =>
      import('./features/rolls/components/rolls-page.component').then(
        (m) => m.RollsPageComponent
      ),
  },
];
