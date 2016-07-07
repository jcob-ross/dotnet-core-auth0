import {
  it,
  describe,
  async,
  inject,
  beforeEachProviders
} from '@angular/core/testing';
import { provide } from '@angular/core';
import { TestComponentBuilder } from '@angular/compiler/testing';
import { BaseRequestOptions, Http } from '@angular/http';
import { MockBackend } from '@angular/http/testing';

import { AuthHttp, AuthConfig } from 'angular2-jwt';

import { ApiService } from '../shared';

import { ProfileComponent } from './profile.component';

describe('Profile Component', () => {
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
    ProfileComponent
  ]);

  it('ngOnInit should call getUserData', async(inject([TestComponentBuilder], (tcb: TestComponentBuilder) => {
    tcb.createAsync(ProfileComponent).then((fixture) => {
      fixture.detectChanges();
      spyOn(fixture.componentInstance, 'getUserData');
      fixture.componentInstance.ngOnInit();

      expect(fixture.componentInstance.getUserData).toHaveBeenCalledTimes(1);
    });
  })));

  it('profile$ be defined after load', async(inject([TestComponentBuilder], (tcb: TestComponentBuilder) => {
    tcb.createAsync(ProfileComponent).then((fixture) => {
      fixture.detectChanges();

      expect(fixture.componentInstance.profile$).not.toBeUndefined();
    });
  })));

});
