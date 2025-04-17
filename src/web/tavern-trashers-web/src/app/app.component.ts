import { Component } from '@angular/core';
import { LayoutComponent } from './shared/layout/layout.component';
import { RouterOutlet } from '@angular/router';
import { AuthFacade } from './state/auth/auth.facade';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LayoutComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'tavern-trashers-web';

  constructor(private auth: AuthFacade) {
    this.auth.tryAutoLogin();
  }
}
