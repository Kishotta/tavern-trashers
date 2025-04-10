import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { AuthState } from '../../state/auth/auth.reducer';
import { map, Observable } from 'rxjs';
import { selectAuth } from '../../state/auth/auth.selectors';
import { AsyncPipe } from '@angular/common';
import { SideNavComponent } from '../side-nav/side-nav.component';
import { MainNavComponent } from "../main-nav/main-nav.component";

@Component({
  selector: 'app-layout',
  imports: [AsyncPipe, SideNavComponent, MainNavComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {
  private auth$: Observable<AuthState>;

  constructor(private store: Store) {
    this.auth$ = this.store.select(selectAuth);
  }

  get userImage() {
    return this.auth$.pipe(map((auth) => auth.user?.email));
  }

  get userName$() {
    return this.auth$.pipe(
      map((auth) => `${auth.user?.given_name} ${auth.user?.family_name}`)
    );
  }
}
