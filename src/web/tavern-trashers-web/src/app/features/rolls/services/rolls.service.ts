import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Roll } from '../models/roll.model';

@Injectable({ providedIn: 'root' })
export class RollsService {
  constructor(private http: HttpClient) {}

  getAllRolls(): Observable<Roll[]> {
    return this.http.get<Roll[]>('/api/dice/rolls');
  }
}
