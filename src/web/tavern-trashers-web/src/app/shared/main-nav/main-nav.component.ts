import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable, map, of, switchMap } from 'rxjs';
import { AuthState } from '../../features/auth/store/auth.reducer';
import { selectAuth } from '../../features/auth/store/auth.selectors';
import { AsyncPipe } from '@angular/common';
import { AuthFacade } from '../../features/auth/services/auth.facade';
import { Router } from '@angular/router';
import { AvatarComponent } from '../../common/avatar/components/avatar/avatar.component';
import { selectBreakpoint } from '../../state/layout/layout.selectors';
import { DropdownComponent } from '../../common/dropdown/components/dropdown/dropdown.component';
import { DropdownItemComponent } from "../../common/dropdown/components/dropdown-item/dropdown-item.component";
import { DropdownDividerComponent } from "../../common/dropdown/components/dropdown-divider/dropdown-divider.component";
import { NotificationsComponent } from "../../features/notifications/components/notifications/notifications.component";

@Component({
  selector: 'app-main-nav',
  imports: [AsyncPipe, AvatarComponent, DropdownComponent, DropdownItemComponent, DropdownDividerComponent, NotificationsComponent],
  templateUrl: './main-nav.component.html',
  styleUrl: './main-nav.component.css',
})
export class MainNavComponent {
  private auth$: Observable<AuthState>;
  breakpoint$: Observable<string>;

  constructor(
    private store: Store,
    private auth: AuthFacade,
    private router: Router
  ) {
    this.auth$ = this.auth.state$;
    this.breakpoint$ = this.store.select(selectBreakpoint);
  }

  get isLoggedIn$() {
    return this.auth$.pipe(map((auth) => auth.user !== null));
  }

  get userProfile$() {
    return this.auth$.pipe(
      switchMap((auth: AuthState) => {
        if (auth.user === null) {
          return of(null);
        }
        return of({
          name: auth.user.name,
          detail: auth.user.email,
          imageSrc: `https://api.dicebear.com/9.x/avataaars-neutral/svg?seed=${auth.user?.name}&scale=80&radius=20&backgroundColor=ae5d29,f8d25c,fd9841,b6e3f4,c0aede,d1d4f9,ffd5dc,d08b5b`,
        });
      })
    );
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
