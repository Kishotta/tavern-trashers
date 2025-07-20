import { computed, signal } from '@angular/core';
import { Component, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Roll } from '../models/roll.model';
import * as RollsActions from '../store/rolls.actions';
import * as RollsSelectors from '../store/rolls.selectors';
import { AsyncPipe, NgClass } from '@angular/common';
import { RollsEffects } from '../store/rolls.effects';

// Helper types for parsed dice expressions
type DiceToken =
  | { type: 'die'; size: string; count: number }
  | { type: 'number'; value: number }
  | { type: 'operator'; op: string };

function parseDiceExpression(expr: string): DiceToken[] {
  // Very basic parser for expressions like 2d6+3-d4
  const tokens: DiceToken[] = [];
  const regex = /(\d*)d(\d+|f)|[+\-*/()]|\d+/gi;
  let match;
  let lastIndex = 0;
  while ((match = regex.exec(expr)) !== null) {
    if (match[0].includes('d')) {
      // Dice
      const count = match[1] ? parseInt(match[1], 10) : 1;
      const size = match[2];
      tokens.push({ type: 'die', size, count });
    } else if (/^\d+$/.test(match[0])) {
      tokens.push({ type: 'number', value: parseInt(match[0], 10) });
    } else if (/^[+\-*/()]$/.test(match[0])) {
      tokens.push({ type: 'operator', op: match[0] });
    }
    lastIndex = regex.lastIndex;
  }
  return tokens;
}

@Component({
  selector: 'tt-rolls-page',
  templateUrl: './rolls-page.component.html',
  styleUrls: ['./rolls-page.component.css'],
  imports: [AsyncPipe, NgClass],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RollsPageComponent implements OnInit {
  // Helper to create an array of length n for template iteration
  counter(n: number): number[] {
    return Array.from({ length: n }, (_, i) => i);
  }
  rolls$: Observable<Roll[]>;
  loading$: Observable<boolean>;
  error$: Observable<any>;

  constructor(private store: Store) {
    this.rolls$ = this.store.select(RollsSelectors.selectAllRolls).pipe(
      // Sort rolls by rolledAtUtc descending (most recent first)
      map((rolls) =>
        [...rolls].sort(
          (a, b) =>
            new Date(b.rolledAtUtc).getTime() -
            new Date(a.rolledAtUtc).getTime()
        )
      )
    );
    this.loading$ = this.store.select(RollsSelectors.selectRollsLoading);
    this.error$ = this.store.select(RollsSelectors.selectRollsError);
  }

  parseDiceExpression = parseDiceExpression;

  ngOnInit(): void {
    this.store.dispatch(RollsActions.loadRolls());
  }

  isKept(
    die: { value: number; size: string },
    keptRolls: { value: number; size: string }[]
  ): boolean {
    return keptRolls.some((k) => k.value === die.value && k.size === die.size);
  }

  // Helper to get the correct die from rawRolls for a given die token and index
  rawRollIndex(
    rawRolls: { value: number; size: string }[],
    token: { size: string },
    i: number
  ): number {
    let count = 0;
    for (let idx = 0; idx < rawRolls.length; idx++) {
      if (rawRolls[idx].size === token.size) {
        if (count === i) return idx;
        count++;
      }
    }
    return -1;
  }
}
