export enum ResetTrigger {
  PerRound = 'PerRound',
  ShortRest = 'ShortRest',
  LongRest = 'LongRest',
  Dawn = 'Dawn',
  Manual = 'Manual',
}

export enum ResourceDirection {
  Spending = 'Spending',
  Accumulating = 'Accumulating',
}

export interface HitPoints {
  id: string;
  baseMaxHitPoints: number;
  currentHitPoints: number;
  temporaryHitPoints: number;
  maxHitPointReduction: number;
  effectiveMaxHitPoints: number;
}

export interface GenericResource {
  id: string;
  name: string;
  currentUses: number;
  maxUses: number;
  direction: ResourceDirection;
  sourceCategory: string;
  resetTriggers: ResetTrigger[];
}

export interface SpellSlotLevel {
  level: number;
  currentUses: number;
  maxUses: number;
}

export interface SpellSlotPool {
  id: string;
  kind: string;
  levels: SpellSlotLevel[];
}

export interface DeathSavingThrows {
  id: string;
  successes: number;
  failures: number;
}

export interface Character {
  id: string;
  name: string;
  level: number;
  ownerId: string;
  campaignId: string;
  conditions: number;
  hitPoints: HitPoints;
  deathSavingThrows: DeathSavingThrows;
  genericResources: GenericResource[];
  spellSlotPools: SpellSlotPool[];
}

export interface CreateCharacterRequest {
  name: string;
  level: number;
  campaignId: string;
}
