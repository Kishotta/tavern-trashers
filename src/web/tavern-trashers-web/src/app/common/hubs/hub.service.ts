import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class HubService implements OnDestroy {
  private connection: signalR.HubConnection | null = null;

  connect(accessTokenFactory: () => string): Observable<void> {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/api/hubs/tavern-trashers', {
        accessTokenFactory,
      })
      .withAutomaticReconnect()
      .build();

    return new Observable<void>((subscriber) => {
      this.connection!.start()
        .then(() => {
          subscriber.next();
          subscriber.complete();
        })
        .catch((err) => subscriber.error(err));
    });
  }

  joinGroup(groupName: string): Observable<void> {
    return new Observable<void>((subscriber) => {
      this.connection!.invoke('JoinGroupAsync', groupName)
        .then(() => {
          subscriber.next();
          subscriber.complete();
        })
        .catch((err) => subscriber.error(err));
    });
  }

  leaveGroup(groupName: string): Observable<void> {
    return new Observable<void>((subscriber) => {
      this.connection!.invoke('LeaveGroupAsync', groupName)
        .then(() => {
          subscriber.next();
          subscriber.complete();
        })
        .catch((err) => subscriber.error(err));
    });
  }

  on<T>(method: string): Observable<T> {
    return new Observable<T>((subscriber) => {
      const handler = (data: T) => subscriber.next(data);
      this.connection!.on(method, handler);
      return () => this.connection?.off(method, handler);
    });
  }

  disconnect(): void {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }
  }

  ngOnDestroy(): void {
    this.disconnect();
  }
}
