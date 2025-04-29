import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Invitation } from '../store/invitations.state';

@Injectable({
  providedIn: 'root',
})
export class InvitationsService {
  constructor(private http: HttpClient) {}

  getInvitations() {
    return this.http.get<Invitation[]>('/api/my/invitations');
  }
}
