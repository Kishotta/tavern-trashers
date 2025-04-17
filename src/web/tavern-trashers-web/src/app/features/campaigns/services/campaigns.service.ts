import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Campaign } from '../store/campaigns.reducer';

@Injectable({
  providedIn: 'root',
})
export class CampaignsService {
  constructor(private http: HttpClient) {}

  getCampaigns(): Observable<Campaign[]> {
    return this.http.get<Campaign[]>('/api/campaigns');
  }
}
