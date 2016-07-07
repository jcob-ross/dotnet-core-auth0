import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/Observable/of';
import 'rxjs/add/operator/catch';

import { ApiService, IUserData, IClaim } from '../shared';

@Component({
  selector: 'my-profile',
  templateUrl: './profile.component.html',
  styles : [`
    table {
      width: 90%;
      text-align: start;
    }
    table th {
      font-weight: bold;
      border-bottom: 1px dotted white;
    }
  `]
})
export class ProfileComponent implements OnInit {
  profile$: Observable<IUserData>;
  constructor(private api: ApiService) {
  }
  ngOnInit() {
    this.getUserData();
  }

  getUserData() {
    this.profile$ = this.api.getUserData()
      .catch(err => {
        console.error(err);
        return Observable.of({ claims: [{type: 'error occured', value: `(${err.status} ${err.statusText})`}] });
      })
      .map((res: any) => res.json ? res.json() : res);
  }
}
