import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable, map } from 'rxjs';
import { AuthState } from '../../state/auth/auth.reducer';
import { selectAuth } from '../../state/auth/auth.selectors';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-main-nav',
  imports: [AsyncPipe],
  templateUrl: './main-nav.component.html',
  styleUrl: './main-nav.component.css',
})
export class MainNavComponent {
  private auth$: Observable<AuthState>;

  constructor(private store: Store) {
    this.auth$ = this.store.select(selectAuth);
  }

  get userImage$() {
    return this.userName$.pipe(
      map(
        (userName) =>
          `https://api.dicebear.com/9.x/initials/svg?seed=${encodeURIComponent(
            userName
          )}`
      )
    );
  }

  get userName$() {
    return this.auth$.pipe(
      map((auth) => `${auth.user?.given_name} ${auth.user?.family_name}`)
    );
  }
}
