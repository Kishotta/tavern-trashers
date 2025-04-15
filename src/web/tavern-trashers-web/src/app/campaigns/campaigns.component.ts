import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ActivatedRoute,
  RouterOutlet,
  Router,
  NavigationEnd,
} from '@angular/router';
import { Store } from '@ngrx/store';
import { NgClass, AsyncPipe } from '@angular/common';
import {
  clearCampaignSelection,
  selectCampaign,
} from '../state/campaigns/campaigns.actions';
import { Campaign } from '../state/campaigns/campaigns.reducer';
import { selectSelectedCampaign } from '../state/campaigns/campaigns.selectors';
import { CampaignListComponent } from './campaign-list/campaign-list.component';

@Component({
  selector: 'app-campaigns',
  imports: [AsyncPipe, NgClass, RouterOutlet, CampaignListComponent],
  templateUrl: './campaigns.component.html',
  styleUrl: './campaigns.component.css',
})
export class CampaignsComponent {
  protected campaigns$!: Observable<Campaign[] | null>;
  protected campaignSelected: boolean = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private store: Store
  ) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        const campaignId = this.route.firstChild?.snapshot.paramMap.get('id');
        if (!campaignId) {
          this.store.dispatch(clearCampaignSelection());
        }
      }
    });

    // this.route.paramMap.subscribe((params) => {
    //   const campaignId = params.get('id');
    //   if (!campaignId) {
    //     this.store.dispatch(clearCampaignSelection());
    //   } else {
    //     this.store.dispatch(selectCampaign({ campaignId }));
    //   }
    // });

    this.store.select(selectSelectedCampaign).subscribe((selectedCampaign) => {
      this.campaignSelected = !!selectedCampaign;
    });
  }
}
