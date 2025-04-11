import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Campaign,
  CampaignsState,
} from '../../state/campaigns/campaigns.reducer';
import { Store } from '@ngrx/store';
import { ActivatedRoute, Router } from '@angular/router';
import {
  clearCampaignSelection,
  loadCampaigns,
  selectCampaign,
} from '../../state/campaigns/campaigns.actions';
import { ProblemDetails } from '../../state/problemDetails';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-campaign-list',
  imports: [AsyncPipe],
  templateUrl: './campaign-list.component.html',
  styleUrl: './campaign-list.component.css',
})
export class CampaignListComponent implements OnInit {
  campaigns$!: Observable<Campaign[]>;
  selectedCampaignId$!: Observable<string | null>;
  error$!: Observable<ProblemDetails | null>;

  constructor(
    private store: Store<{ campaigns: CampaignsState }>,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.store.dispatch(loadCampaigns());

    this.route.firstChild?.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) this.store.dispatch(selectCampaign({ campaignId: id }));
      else this.store.dispatch(clearCampaignSelection());
    });

    this.campaigns$ = this.store.select((state) => state.campaigns.campaigns);
    this.selectedCampaignId$ = this.store.select(
      (state) => state.campaigns.selectedCampaignId
    );
    this.error$ = this.store.select((state) => state.campaigns.error);
  }
}
