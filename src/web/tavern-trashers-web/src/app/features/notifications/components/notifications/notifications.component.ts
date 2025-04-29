import { Component } from '@angular/core';
import { DropdownComponent } from '../../../../common/dropdown/components/dropdown/dropdown.component';
import { InvitationsStore } from '../../store/invitations.store';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'tt-notifications',
  imports: [DropdownComponent, AsyncPipe],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.css',
  providers: [InvitationsStore],
})
export class NotificationsComponent {
  invitations$;

  constructor(private readonly InvitationsStore: InvitationsStore) {
    this.InvitationsStore.loadInvitations();
    this.invitations$ = this.InvitationsStore.invitations$;
  }
}
