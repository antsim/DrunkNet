import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '@auth0/auth0-angular';
import { BloodAlcoholLevel } from '../models/bloodalcohollevel.model';
import { ChartConfiguration, ChartEvent, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { map, Subscription, combineLatest, timer, first, merge, forkJoin, zip } from 'rxjs';
import { DrinkService } from '../services/drink.service';

@Component({
  selector: 'app-bac',
  templateUrl: './bac.component.html',
  styleUrls: ['./bac.component.css'],
})
export class BacComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription = new Subscription();

  public bacHistory: BloodAlcoholLevel[] = [];
  public lineChartType: ChartType = 'line';

  public lineChartData: ChartConfiguration['data'] = {
    datasets: [],
    labels: []
  };

  public lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    elements: {
      line: {
        tension: 0.5
      }
    },
    scales: {
      x: {},
      'y-axis-0':
        {
          beginAtZero: true,
          position: 'left',
        }
    },

    plugins: {
      legend: { display: true }
    }
  };

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  constructor(
    public auth: AuthService,
    private http: HttpClient,
    private drinkService: DrinkService) {}

  ngOnInit(): void {
    this.auth.user$.subscribe(user => user?.name)
    this.subscriptions.add(merge(this.drinkService.drinkConsumed$, timer(0, 15 * 60 * 1000))
      .subscribe(result => {
        this.getBacHistory();
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public getBac() {
    this.http
      .get<BloodAlcoholLevel>(encodeURI('bac'))
      .subscribe((value) => console.log('Current BAC', value));
  }

  public getBacHistory() {
    this.auth.isAuthenticated$.pipe(first()).subscribe((isAuthenticated: boolean) => {
      if (isAuthenticated) {
        this.http
        .get<BloodAlcoholLevel[]>(encodeURI('bachistory'))
        .pipe(
          map((result) => {
            result.map(r => r.bacUpdated = new Date(r.bacUpdated));
            return result;
          })
        )
        .subscribe((value) => {
          this.bacHistory = value;
          const bac = value.slice(-1).pop()?.bac;

          if (bac) {
            this.drinkService.currentBac$.next(bac);
          }
          
          this.lineChartData = {
            datasets: [
              {
                data: this.bacHistory.map(bac => bac.bac),
                label: 'BAC',
                fill: 'origin'
              }
            ],
            labels: this.bacHistory.map(bac => `${bac.bacUpdated.getHours()}.${(bac.bacUpdated.getMinutes() < 10 ? '0' : '') + bac.bacUpdated.getMinutes()}`)
          };

          this.chart?.update();
        });
      }
    })

    
  }
}
