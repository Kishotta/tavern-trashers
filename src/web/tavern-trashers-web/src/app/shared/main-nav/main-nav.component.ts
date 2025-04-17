import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable, map } from 'rxjs';
import { AuthState } from '../../features/auth/store/auth.reducer';
import { selectAuth } from '../../features/auth/store/auth.selectors';
import { AsyncPipe } from '@angular/common';
import { AuthFacade } from '../../features/auth/services/auth.facade';
import { DropdownComponent } from '../../common/dropdown/dropdown.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-main-nav',
  imports: [AsyncPipe, DropdownComponent],
  templateUrl: './main-nav.component.html',
  styleUrl: './main-nav.component.css',
})
export class MainNavComponent {
  private auth$: Observable<AuthState>;

  constructor(private auth: AuthFacade, private router: Router) {
    this.auth$ = this.auth.state$;
  }

  get isLoggedIn$() {
    return this.auth$.pipe(map((auth) => auth.user !== null));
  }

  get userName$() {
    return this.auth$.pipe(map((auth) => auth.user?.name));
  }

  get userImage$() {
    return this.userName$.pipe(
      map(
        (name) =>
          `https://api.dicebear.com/9.x/avataaars-neutral/svg?seed=${name}&scale=90&radius=20&backgroundColor=ae5d29,f8d25c,fd9841,b6e3f4,c0aede,d1d4f9,ffd5dc,d08b5b&eyebrows=angryNatural,defaultNatural,flatNatural,frownNatural,raisedExcitedNatural,sadConcernedNatural,upDownNatural&eyes=default,eyeRoll,side,squint,surprised&mouth=default,disbelief,eating,grimace,sad,serious,smile,twinkle,screamOpen,concerned,tongue`
      )
    );
  }

  register() {
    this.router.navigate(['/register']);
  }

  login() {
    this.auth.login();
  }

  logout() {
    this.auth.logout();
  }
}
