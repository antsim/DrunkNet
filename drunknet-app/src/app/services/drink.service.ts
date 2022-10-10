import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { BloodAlcoholLevel } from '../models/bloodalcohollevel.model';
import { Drink } from '../models/drink.model'
import { Consumption } from '../models/consumption.model';

@Injectable({
  providedIn: 'root'
})
export class DrinkService {

  constructor(private http: HttpClient) { }

  public drinkConsumed$: Subject<boolean> = new Subject<boolean>();
  public currentBac$: Subject<number> = new Subject<number>();

  public addDrink$(drink: Drink): Observable<BloodAlcoholLevel> {
    return this.http
      .post<BloodAlcoholLevel>(encodeURI('drink'), drink);
  }

  public getDrinkList$(): Observable<Consumption[]> {
    return this.http
      .get<Consumption[]>(encodeURI('drinkList'));
  }

  public deleteDrink$(drink: Consumption) {
    const uri = `drink/${drink.consumptionId}`;
    console.log('delete clicked', uri, drink);
    return this.http
      .delete(encodeURI(uri));
  }
}
