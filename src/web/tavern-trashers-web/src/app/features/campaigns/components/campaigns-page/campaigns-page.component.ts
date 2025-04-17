import { Component } from '@angular/core';
import {
  Router,
  ActivatedRoute,
  NavigationEnd,
  RouterOutlet,
} from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { clearCampaignSelection } from '../../store/campaigns.actions';
import { Campaign } from '../../store/campaigns.reducer';
import { selectSelectedCampaign } from '../../store/campaigns.selectors';
import { NgClass } from '@angular/common';
import { CampaignListComponent } from '../campaign-list/campaign-list.component';

@Component({
  selector: 'app-campaigns-page',
  imports: [NgClass, RouterOutlet, CampaignListComponent],
  templateUrl: './campaigns-page.component.html',
  styleUrl: './campaigns-page.component.css',
})
export class CampaignsPageComponent {
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

    this.store.select(selectSelectedCampaign).subscribe((selectedCampaign) => {
      this.campaignSelected = !!selectedCampaign;
    });
  }
}
