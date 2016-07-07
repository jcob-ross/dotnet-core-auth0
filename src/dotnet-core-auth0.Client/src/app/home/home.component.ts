import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import 'rxjs/add/Observable/of';

import { ApiService } from '../shared';

export interface IPingResponse {
  message: string;
}

@Component({
  selector: 'my-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  message: IPingResponse = { message: '' };
  sub: Subscription;

  constructor(private api: ApiService) {
  }

  onPingClick() {
    this.handleRequest(this.api.ping());
  }

  onSecurePingClick() {
    this.handleRequest(this.api.securePing());
  }

  onOwnerPingClick() {
    this.handleRequest(this.api.ownerOnlyPing());
  }

  ngOnInit() {
    console.log('Hello Home');
  }

  ngOnDestroy() {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }

  private handleRequest(request) {
    if (this.sub && !this.sub.isUnsubscribed) {
      this.sub.unsubscribe();
    }
    this.sub = request.catch(err => {
      console.error(err);
      return Observable.of({ message: `error occured - ${err.status ? err.status : err} ${err.statusText ? err.statusText : ''}` });
    }).subscribe(res => {
      this.message = res.json ? res.json() : res;
    });
  }
}
