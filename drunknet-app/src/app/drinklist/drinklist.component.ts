import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription, switchMap, timer, forkJoin, startWith, first } from 'rxjs';
import { DrinkService } from '../services/drink.service';
import { Consumption } from '../models/consumption.model';

@Component({
  selector: 'app-drinklist',
  templateUrl: './drinklist.component.html',
  styleUrls: ['./drinklist.component.css']
})
export class DrinklistComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription = new Subscription();
  public drinks: Consumption[] = [];
  constructor(private drinkService: DrinkService) { }

  ngOnInit(): void {
    this.subscriptions.add(this.drinkService.drinkConsumed$.pipe(startWith(false), switchMap(() => {
      return this.drinkService.getDrinkList$();
    })).subscribe(list => {
      this.drinks = list;
    }));
  }

  public deleteDrink(drink: Consumption): void {
    this.drinkService.deleteDrink$(drink).pipe(first()).subscribe(() => {
      // Emitting next value for drinkConsumed causes ui refresh
      this.drinkService.drinkConsumed$.next(true);
    })
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
