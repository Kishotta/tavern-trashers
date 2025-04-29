import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { Invitation, InvitationsState } from './invitations.state';
import { exhaustMap, switchMap, tap } from 'rxjs';
import { InvitationsService } from '../services/invitations.service';
import { tapResponse } from '@ngrx/operators';

@Injectable()
export class InvitationsStore extends ComponentStore<InvitationsState> {
  constructor(private invitationsService: InvitationsService) {
    super({ invitations: [], loading: false, error: null });
  }

  // Selectors
  readonly invitations$ = this.select((state) => state.invitations);
  readonly loading$ = this.select((state) => state.loading);
  readonly error$ = this.select((state) => state.error);

  // Updaters
  private readonly setInvitations = this.updater<Invitation[]>(
    (state, invitations) => ({
      ...state,
      invitations,
      loading: false,
      error: null,
    })
  );

  // Effects
  readonly loadInvitations = this.effect<void>((trigger$) => {
    return trigger$.pipe(
      tap(() => {
        this.patchState({ loading: true, error: null });
      }),
      exhaustMap(() =>
        this.invitationsService.getInvitations().pipe(
          tapResponse({
            next: (invitations) => this.setInvitations(invitations),
            error: (error) =>
              this.patchState({ loading: false, error: error?.toString() }),
          })
        )
      )
    );
  });
}
