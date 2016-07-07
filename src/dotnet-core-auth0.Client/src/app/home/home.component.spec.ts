import {
  it,
  inject,
  describe,
  beforeEachProviders,
} from '@angular/core/testing';
import { provide } from '@angular/core';
import { BaseRequestOptions, Http } from '@angular/http';
import { MockBackend } from '@angular/http/testing';

import { AuthHttp, AuthConfig } from 'angular2-jwt';
import { ApiService } from '../shared';
import { HomeComponent } from './home.component';

describe('Home Component', () => {
  // provide our implementations or mocks to the dependency injector
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
    ApiService,
    HomeComponent
  ]);

  it('should log on ngOnInit', inject([HomeComponent], (home) => {
    spyOn(console, 'log');
    expect(console.log).not.toHaveBeenCalled();

    home.ngOnInit();
    expect(console.log).toHaveBeenCalledWith('Hello Home');
  }));

});
