import { AsyncPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import {
  bulkRestoreResources,
  applyResource,
  loadCharacters,
  restoreResource,
  useResource,
} from '../../../characters/store/characters.actions';
import {
  selectCharacters,
  selectCharactersLoading,
} from '../../../characters/store/characters.selectors';
import { Character, ResetTrigger } from '../../../characters/models/character.model';
import { PageHeadingComponent } from '../../../../common/page/components/page-heading/page-heading.component';

@Component({
  selector: 'app-dm-overview',
  imports: [AsyncPipe, PageHeadingComponent],
  templateUrl: './dm-overview.component.html',
  styleUrl: './dm-overview.component.css',
})
export class DmOverviewComponent implements OnInit {
  protected characters$: Observable<Character[]>;
  protected loading$: Observable<boolean>;
  protected campaignId!: string;

  protected readonly resetTriggers: { label: string; value: ResetTrigger }[] = [
    { label: 'Long Rest', value: ResetTrigger.LongRest },
    { label: 'Short Rest', value: ResetTrigger.ShortRest },
    { label: 'Per Round', value: ResetTrigger.PerRound },
    { label: 'Dawn', value: ResetTrigger.Dawn },
  ];

  constructor(
    private route: ActivatedRoute,
    private store: Store
  ) {
    this.characters$ = this.store.select(selectCharacters);
    this.loading$ = this.store.select(selectCharactersLoading);
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (!id) {
        return;
      }
      this.campaignId = id;
      this.store.dispatch(loadCharacters({ campaignId: this.campaignId }));
    });
  }

  onBulkRestore(trigger: ResetTrigger): void {
    this.store.dispatch(bulkRestoreResources({ campaignId: this.campaignId, trigger }));
  }

  onUseResource(characterId: string, resourceId: string): void {
    this.store.dispatch(useResource({ characterId, resourceId }));
  }

  onApplyResource(characterId: string, resourceId: string): void {
    this.store.dispatch(applyResource({ characterId, resourceId }));
  }

  onRestoreResource(characterId: string, resourceId: string): void {
    this.store.dispatch(restoreResource({ characterId, resourceId }));
  }
}
