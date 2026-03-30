export interface Character {
  id: string;
  name: string;
  level: number;
  ownerId: string;
  campaignId: string;
}

export interface CreateCharacterRequest {
  name: string;
  level: number;
  campaignId: string;
}
