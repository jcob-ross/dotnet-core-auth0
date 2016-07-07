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

import { AuthHttp, AuthConfig } from 'angular2-jwt';
import { ApiService } from './api.service';

describe('Api Service', () => {

  beforeEachProviders(() => [
    MockBackend,
    BaseRequestOptions,

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
    ApiService
  ]);

  it('title should be Aungular 2', inject([ApiService], (api: ApiService) => {
    expect(api.title).toBe('Angular 2');
  }));

});
