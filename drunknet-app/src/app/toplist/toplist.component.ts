import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { Subscription, switchMap, timer } from 'rxjs';
import { TopListUser } from '../models/toplistuser.model';
import { UserService } from '../services/user.service';
import { DrinkService } from '../services/drink.service';

@Component({
  selector: 'app-toplist',
  templateUrl: './toplist.component.html',
  styleUrls: ['./toplist.component.css']
})
export class ToplistComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription = new Subscription();
  public topList: TopListUser[] = [];
  public refreshedAt: Date = new Date();

  constructor(
    private userService: UserService,
    private auth: AuthService,
    private drinkService: DrinkService
  ) { }

  ngOnInit(): void {
    this.subscriptions.add(this.drinkService.drinkConsumed$.pipe(switchMap(() => {
      return this.userService.getTopList$();
    })).subscribe(list => {
      this.topList = list
      this.refreshedAt = new Date();
    }));

    this.subscriptions.add(this.userService.getTopList$().subscribe(list => {
      this.topList = list;
      this.refreshedAt = new Date();
    }));

    this.subscriptions.add(timer(0, 300000).pipe(switchMap(() => {
      return this.userService.getTopList$();
    })).subscribe(result => {
      this.topList = result;
      this.refreshedAt = new Date();
    }));
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
