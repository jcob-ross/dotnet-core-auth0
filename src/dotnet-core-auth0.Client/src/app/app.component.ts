import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router } from '@angular/router';

import { ApiService, AuthService } from './shared';

import '../style/app.scss';

/*
 * App Component
 * Top Level Component
 */
@Component({
  selector: 'my-app',
  providers: [ApiService, AuthService],
  directives: [...ROUTER_DIRECTIVES],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  url = 'https://github.com/jcob-ross/dotnet-core-auth0';
  urlText = '\u2661 Angular 2 ASP.NET core with Auth0';
  userName: string = null;
  userPictureUrl: string = null;

  constructor(private router: Router, private api: ApiService, private auth: AuthService) {
  }

  ngOnInit() {
    this.getUserData();
  }

  isLoggedIn(): boolean {
    return this.auth.isLoggedIn;
  }

  onLogInClick() {
    this.auth.login().then(user => {
      this.userName = user.name;
      this.userPictureUrl = user.hasPicture ? user.pictureUrl : null;
    }, err => {
      this.clearUserData();
    });
  }

  onLogOutClick() {
    this.auth.logout();
    this.clearUserData();
  }

  private clearUserData() {
    this.userName = null;
    this.userPictureUrl = null;
    this.router.navigate(['/']);
  }

  private getUserData() {
    if (this.auth.isLoggedIn) {
      this.userName = this.auth.currentUser.name;
      this.userPictureUrl = this.auth.currentUser.hasPicture ? this.auth.currentUser.pictureUrl : null;
    }
  }
}
