import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './auth.service';
import { AsyncPipe, JsonPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { selectAuth } from './state/auth/auth.selectors';
import { Observable } from 'rxjs';
import { login, logout, tryAutoLogin } from './state/auth/auth.actions';
import { LayoutComponent } from './shared/layout/layout.component';

@Component({
  selector: 'app-root',
  imports: [AsyncPipe, JsonPipe, RouterLink, RouterOutlet, LayoutComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'tavern-trashers-web';

  public auth$: Observable<any>;

  constructor(protected authService: AuthService, private store: Store) {
    this.auth$ = this.store.select(selectAuth);
  }

  ngOnInit(): void {
    this.store.dispatch(tryAutoLogin());
  }

  login(): void {
    this.store.dispatch(login());
  }
  logout(): void {
    this.store.dispatch(logout());
  }
}
