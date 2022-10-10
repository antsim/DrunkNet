import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TopListUser } from '../models/toplistuser.model';
import { UserProfile } from '../models/userprofile.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

  public getProfile$() : Observable<UserProfile> {
    return this.http
      .get<UserProfile>(encodeURI('profile'));
  }

  public updateProfile$(profile: UserProfile): Observable<UserProfile> {
    return this.http
      .post<UserProfile>(encodeURI('profile'), profile);
  }

  public getTopList$(): Observable<TopListUser[]> {
    return this.http
      .get<TopListUser[]>(encodeURI('toplist'));
  }
}
