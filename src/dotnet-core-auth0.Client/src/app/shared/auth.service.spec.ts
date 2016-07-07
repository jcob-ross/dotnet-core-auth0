import {
  it,
  describe,
  expect,
  inject,
  beforeEachProviders
} from '@angular/core/testing';
import { provide } from '@angular/core';
import { BaseRequestOptions, Http } from '@angular/http';
import { MockBackend } from '@angular/http/testing';

import { AuthService } from './auth.service';

describe('Auth Service', () => {

  beforeEachProviders(() => [
    MockBackend,
    BaseRequestOptions,

    provide(Http, {
      useFactory: (backend, defaultOptions) => {
        return new Http(backend, defaultOptions);
      },
      deps: [MockBackend, BaseRequestOptions]
    }),
    AuthService
  ]);

  it('isLoggedIn should be false after load', inject([AuthService], (auth: AuthService) => {
    expect(auth.isLoggedIn).toEqual(false);
  }));

  it('current user should be null after load', inject([AuthService], (auth: AuthService) => {
    expect(auth.currentUser).toEqual(null);
  }));

});
