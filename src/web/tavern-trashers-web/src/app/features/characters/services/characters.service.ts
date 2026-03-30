import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Character, CreateCharacterRequest } from '../models/character.model';

@Injectable({
  providedIn: 'root',
})
export class CharactersService {
  constructor(private http: HttpClient) {}

  getCharactersByCampaign(campaignId: string): Observable<Character[]> {
    return this.http.get<Character[]>(`/api/campaigns/${campaignId}/characters`);
  }

  createCharacter(request: CreateCharacterRequest): Observable<Character> {
    return this.http.post<Character>('/api/characters', request);
  }
}
