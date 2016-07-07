import {
  provideRouter,
  RouterConfig,
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot } from '@angular/router';

import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/Observable/of';

import { HomeComponent } from './home';
import { AboutComponent } from './about';
import { ProfileComponent } from './profile';
import { AuthService } from './shared';


@Injectable()
export class CanActivateProfile implements CanActivate {
  constructor(private auth: AuthService) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    if (this.auth.isLoggedIn) {
      return Observable.of(true);
    }
    return Observable.of(false);
  }
};

export const routes: RouterConfig = [
  { path: '', component: HomeComponent },
  { path: 'about', component: AboutComponent },

  { path: 'profile', component: ProfileComponent, canActivate: [ CanActivateProfile ] }
];

export const APP_ROUTER_PROVIDERS = [
  CanActivateProfile,
  provideRouter(routes)
];
