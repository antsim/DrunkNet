import { Component, OnInit } from '@angular/core';
import { FormBuilder, UntypedFormControl } from '@angular/forms';
import { Drink } from '../models/drink.model';
import { DrinkService } from '../services/drink.service';

@Component({
  selector: 'app-drink',
  templateUrl: './drink.component.html',
  styleUrls: ['./drink.component.css']
})
export class DrinkComponent implements OnInit {
  public rawData = new UntypedFormControl();

  constructor(
    private drinkService: DrinkService
  ) { }

  ngOnInit(): void {
  }

  public onSubmitAddDrink(): void {
    const drink: Drink = new Drink();
    drink.rawData = this.rawData.value;

    this.drinkService.addDrink$(drink).subscribe(() => {
      this.rawData.reset();
      this.drinkService.drinkConsumed$.next(true);
    });
  }

}
