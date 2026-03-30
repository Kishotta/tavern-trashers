import { AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { selectCampaign } from '../../store/campaigns.actions';
import { Campaign } from '../../store/campaigns.reducer';
import { selectSelectedCampaign } from '../../store/campaigns.selectors';
import { PageHeadingComponent } from "../../../../common/page/components/page-heading/page-heading.component";
import { CharacterRosterComponent } from "../../../characters/components/character-roster/character-roster.component";
import { CreateCharacterFormComponent } from "../../../characters/components/create-character-form/create-character-form.component";

@Component({
  selector: 'app-campaign-detail',
  imports: [AsyncPipe, RouterLink, PageHeadingComponent, CharacterRosterComponent, CreateCharacterFormComponent],
  templateUrl: './campaign-detail.component.html',
  styleUrl: './campaign-detail.component.css',
})
export class CampaignDetailComponent {
  campaign$!: Observable<Campaign | null>;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private store: Store<any>
  ) {
    this.route.paramMap.subscribe((params) => {
      const campaignId = params.get('id')!;
      this.store.dispatch(selectCampaign({ campaignId }));
    });

    this.campaign$ = this.store.select(selectSelectedCampaign);
  }

  onBack() {
    this.router.navigate(['campaigns']);
  }
}
