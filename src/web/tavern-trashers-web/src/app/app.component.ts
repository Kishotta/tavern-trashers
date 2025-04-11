import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { tryAutoLogin } from './state/auth/auth.actions';
import { LayoutComponent } from './shared/layout/layout.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LayoutComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'tavern-trashers-web';

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.store.dispatch(tryAutoLogin());
  }
}
