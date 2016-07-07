import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

const API_URL = 'http://localhost:8080/api/';

@Injectable()
export class ApiService {
  title = 'Angular 2';

  constructor(private http: Http, private secureHttp: AuthHttp) {
  }

  ping(): any {
    return this.http.get(`${API_URL}ping`);
  }

  securePing() {
    return this.secureHttp.get(`${API_URL}secured-ping`);
  }

  ownerOnlyPing() {
    return this.secureHttp.get(`${API_URL}owner-only`);
  }

  getUserData() {
    return this.secureHttp.get(`${API_URL}user-data`);
  }
}
