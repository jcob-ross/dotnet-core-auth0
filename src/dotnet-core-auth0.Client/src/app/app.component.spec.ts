import {
  it,
  inject,
  beforeEachProviders
} from '@angular/core/testing';

// to use Translate Service, we need Http, and to test Http we need to mock the backend
import { BaseRequestOptions, Http } from '@angular/http';
import { MockBackend } from '@angular/http/testing';
import { provide } from '@angular/core';
import { Router } from '@angular/router';

import { AuthHttp, AuthConfig } from 'angular2-jwt';

import { ApiService, AuthService } from './shared';
import { AppComponent } from './app.component';

// full router setup http://plnkr.co/edit/0og1Oz?p=info
class StubRouter { }

describe('App Component', () => {

  // provide our implementations or mocks to the dependency injector
  beforeEachProviders(() => [
    MockBackend,
    BaseRequestOptions,

    provide(Router, { useClass: StubRouter }),

    provide(Http, {
      useFactory: (backend, defaultOptions) => {
        return new Http(backend, defaultOptions);
      },
      deps: [MockBackend, BaseRequestOptions]
    }),
    provide(AuthHttp, {
      useFactory: (http) => {
        return new AuthHttp(new AuthConfig(), http);
      },
      deps: [Http]
    }),
    AuthService,
    ApiService,
    AppComponent
  ]);

  it('should have an url', inject([AppComponent], (app: AppComponent) => {
    expect(app.url).toEqual('https://github.com/jcob-ross/dotnet-core-auth0');
  }));

});
