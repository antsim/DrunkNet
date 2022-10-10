import { NgModule, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { environment } from 'src/environments/environment';

import localeFi from '@angular/common/locales/fi'
registerLocaleData(localeFi);

import { AuthModule, AuthHttpInterceptor } from '@auth0/auth0-angular';
import { AuthComponent } from './auth/auth.component';
import { BacComponent } from './bac/bac.component';
import { APIInterceptor } from './interceptors/api.interceptor';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { ProfileComponent } from './profile/profile.component';
import { DrinkComponent } from './drink/drink.component';
import { NgChartsModule } from 'ng2-charts';
import { ToplistComponent } from './toplist/toplist.component';
import { DrinklistComponent } from './drinklist/drinklist.component';
import { registerLocaleData } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [AppComponent, AuthComponent, BacComponent, ProfileComponent, DrinkComponent, ToplistComponent, DrinklistComponent],
  imports: [
    BrowserModule,
    ReactiveFormsModule, 
    FormsModule,
    AuthModule.forRoot({
      domain: environment.authDomain,
      clientId: environment.clientId,
      audience: environment.authAudience,
      scope: 'email profile',

      httpInterceptor: {
        allowedList: [
          {
            uri: `${environment.authAudience}*`,
            tokenOptions: {
              audience: environment.authAudience,
              scope: 'email profile'
            },
          },
          {
            uri: `${environment.apiDomain}/*`,
            tokenOptions: {
              audience: environment.apiDomain,
              scope: 'email profile openid',
            },
          },
        ],
      },
    }),
    HttpClientModule,
    BrowserAnimationsModule,
    NgChartsModule,
    NgbModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: APIInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'fi-FI' }
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
