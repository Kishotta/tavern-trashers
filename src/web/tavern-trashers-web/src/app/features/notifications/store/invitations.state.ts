export interface Invitation {
  id: string;
  email: string;
  campaignId: string;
  campaignTitle: string;
  campaignRole: 'Player' | 'DungeonMaster';
}

export interface InvitationsState {
  invitations: Invitation[];
  loading: boolean;
  error: string | null;
}
