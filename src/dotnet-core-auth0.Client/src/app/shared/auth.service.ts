import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { JwtHelper } from 'angular2-jwt';
import { User, IAuth0Profile } from './models';

// imported in script tag
declare var Auth0Lock: any;

const options = {
  domain: 'jcob-ross.eu.auth0.com',
  clientId: 'EG9x8DKRALQKL3rDGYloT7uZCj6ku3bb',
  lockOptions: {
    responseType: 'token',
    authParams: {
      state: 'foo bar state',
      scope: 'openid nickname email'
    }
  }
};


@Injectable()
export class AuthService {
  private _lock = new Auth0Lock(options.clientId, options.domain);
  private _jwtHelper = new JwtHelper();
  private _user: User = null;

  constructor(private http: Http) {
    this.restoreSession();
  }

  /**
   * Shows Auth0 Lock to log user in
   */
  login(): Promise<User> {
    return new Promise<User>((resolve, reject) => {
      this._lock.show(options.lockOptions, (error, profile, token, accessToken, state) => {
        this.lockCallback(resolve, reject, error, profile, token, accessToken, state);
      });
    });
  }

  logout(): void {
    this.clearUserData();
  }

  get isLoggedIn(): boolean {
    return this._user != null;
  }

  get currentUser(): User {
    return this._user;
  }

  private lockCallback(resolve, reject, err, profile: IAuth0Profile, token: string, accessToken: string, state: string): void {
    if (err != null) {
      console.error('login failed with errors', err);
      this.clearUserData();
      reject('login failed with errors');
      return;
    }
    let decodedToken = this._jwtHelper.decodeToken(token);
    let pictureUrl = null;
    if (profile.picture && profile.picture.length > 0) {
      pictureUrl = profile.picture;
    }
    let user = new User(decodedToken.nickname, decodedToken.email, pictureUrl);
    this._user = user;

    localStorage.setItem('id_token', token);
    localStorage.setItem('access_token', accessToken);
    let stringifiedUser = JSON.stringify(user);
    let encoded = window.btoa(encodeURIComponent(stringifiedUser));
    localStorage.setItem('user', encoded);
    resolve(user);
  }

  private restoreSession(): void {
    let userEncoded = localStorage.getItem('user');
    if (!userEncoded || !localStorage.getItem('id_token')) {
      this.clearUserData();
      return;
    }
    if (this._jwtHelper.isTokenExpired(localStorage.getItem('id_token'))) {
      this.clearUserData();
      return;
    }
    let decodedUser = JSON.parse(this.decode(userEncoded));
    this._user = new User(decodedUser._name, decodedUser._email, decodedUser._pictureUrl);
  }

  private clearUserData(): void {
    this._user = null;
    localStorage.removeItem('id_token');
    localStorage.removeItem('access_token');
    localStorage.removeItem('user');
  }

  private decode(text: string): string {
    return decodeURIComponent(window.atob(text));
  }
}
