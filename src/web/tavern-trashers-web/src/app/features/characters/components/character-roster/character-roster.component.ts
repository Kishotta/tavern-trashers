import { AsyncPipe } from '@angular/common';
import { Component, Input, OnChanges } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { Character } from '../../models/character.model';
import { loadCharacters } from '../../store/characters.actions';
import { selectCharacters, selectCharactersLoading } from '../../store/characters.selectors';

@Component({
  selector: 'app-character-roster',
  imports: [AsyncPipe],
  templateUrl: './character-roster.component.html',
  styleUrl: './character-roster.component.css',
})
export class CharacterRosterComponent implements OnChanges {
  @Input() campaignId!: string;

  protected characters$: Observable<Character[]>;
  protected loading$: Observable<boolean>;

  constructor(private store: Store) {
    this.characters$ = this.store.select(selectCharacters);
    this.loading$ = this.store.select(selectCharactersLoading);
  }

  ngOnChanges(): void {
    if (this.campaignId) {
      this.store.dispatch(loadCharacters({ campaignId: this.campaignId }));
    }
  }
}
