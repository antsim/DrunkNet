import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { of, Subscription, switchMap } from 'rxjs';
import { UserService } from '../services/user.service';
import { UserProfile } from '../models/userprofile.model';
import { Gender } from '../models/gender.enum';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit, OnDestroy {

  private subscriptions: Subscription = new Subscription();

  public mustUpdateProfile: boolean = false;

  public genderKeys = Object.values(Gender).filter((o) => typeof o == 'number').map(g => Number(g));
  public weight: number = 55;
  public gender: number = 2;
  public genders = Gender;
  public userName: string = '';

  constructor(
    public auth: AuthService,
    private userService: UserService) { }

  ngOnInit(): void {
    this.subscriptions.add(this.auth.isAuthenticated$.pipe(
      switchMap((isAuthenticated: boolean) => {
        if (isAuthenticated) {
          return this.userService.getProfile$();
        }
        return of();
    })).subscribe(result => {
      if (result.weight == 0 || result.gender == 0 || result.name == '') {
        this.mustUpdateProfile = true;
      }
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public formatLabel(value: number) {
    if (value >= 250) {
      return Math.round(value / 250);
    }

    return value;
  }

  public onSubmitUpdateProfile(): void {
    const user: UserProfile = new UserProfile();
    user.gender = this.gender;
    user.weight = this.weight;
    user.name = this.userName;


    this.userService.updateProfile$(user).subscribe(result => {
      this.weight = 55;
      this.gender = Gender.male;
      this.userName = '';
      this.mustUpdateProfile = false;
    });
  }
}
