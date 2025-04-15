import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { BreakpointObserver } from '@angular/cdk/layout';
import { setBreakpoint } from '../../state/layout/layout.actions';

@Injectable({
  providedIn: 'root',
})
export class LayoutService {
  private readonly MEDIA_QUERIES = {
    mobile: '(max-width: 80rem)',
    desktop: '(min-width: 80rem)',
  };

  constructor(
    private breakpointObserver: BreakpointObserver,
    private store: Store
  ) {
    this.breakpointObserver
      .observe(this.MEDIA_QUERIES.mobile)
      .subscribe((result) => {
        if (result.matches) {
          this.store.dispatch(setBreakpoint({ breakpoint: 'mobile' }));
        }
      });
    this.breakpointObserver
      .observe(this.MEDIA_QUERIES.desktop)
      .subscribe((result) => {
        if (result.matches) {
          this.store.dispatch(setBreakpoint({ breakpoint: 'desktop' }));
        }
      });
  }
}
