export interface ResourceChangedNotification {
  characterId: string;
  characterName: string;
  campaignId: string;
  resourceName: string;
  oldValue: string;
  newValue: string;
  actor: string;
}
