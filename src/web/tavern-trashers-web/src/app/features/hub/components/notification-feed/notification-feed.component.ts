import { Component } from '@angular/core';
import { AsyncPipe, NgClass } from '@angular/common';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { HubNotification } from '../../store/hub.reducer';
import { selectHubNotifications } from '../../store/hub.selectors';

@Component({
  selector: 'app-notification-feed',
  imports: [AsyncPipe, NgClass],
  templateUrl: './notification-feed.component.html',
  styleUrl: './notification-feed.component.css',
})
export class NotificationFeedComponent {
  protected notifications$: Observable<HubNotification[]>;

  constructor(private store: Store) {
    this.notifications$ = this.store.select(selectHubNotifications);
  }

  formatTime(timestamp: string): string {
    const date = new Date(timestamp);
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });
  }
}
