import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { login, logout } from '../state/auth/auth.actions';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent {
  constructor(private store: Store) {}

  login(): void {
    this.store.dispatch(login());
  }

  logout(): void {
    this.store.dispatch(logout());
  }
}
