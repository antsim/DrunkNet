import { Component, OnInit, Inject, OnDestroy } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { DOCUMENT } from '@angular/common';
import { Subscription } from 'rxjs';
import { DrinkService } from '../services/drink.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css'],
})
export class AuthComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription = new Subscription();
  public currentBac: number = 0;

  constructor(
    @Inject(DOCUMENT) public document: Document,
    public auth: AuthService,
    private drinkService: DrinkService 
  ) {}

  ngOnInit(): void {  
    this.subscriptions.add(this.drinkService.currentBac$.subscribe(bac => this.currentBac = bac));
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
