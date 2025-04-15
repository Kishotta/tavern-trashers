import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { Campaign } from '../../state/campaigns/campaigns.reducer';
import {
  selectCampaigns,
  selectSelectedCampaign,
} from '../../state/campaigns/campaigns.selectors';
import { Router, RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { loadCampaigns } from '../../state/campaigns/campaigns.actions';

@Component({
  selector: 'app-campaign-list',
  imports: [AsyncPipe, RouterLink],
  templateUrl: './campaign-list.component.html',
  styleUrl: './campaign-list.component.css',
})
export class CampaignListComponent {
  protected campaigns$: Observable<Campaign[] | null>;
  protected selectedCampaign$: Observable<Campaign | null>;

  constructor(private router: Router, private store: Store) {
    this.store.dispatch(loadCampaigns());

    this.campaigns$ = this.store.select(selectCampaigns);
    this.selectedCampaign$ = this.store.select(selectSelectedCampaign);
  }

  onCampaignSelect(campaign: any) {
    this.router.navigate(['campaigns', campaign.id]);
  }
}
