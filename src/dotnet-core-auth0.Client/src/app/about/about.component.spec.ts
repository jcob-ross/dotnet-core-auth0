import {
  it,
  describe,
  async,
  inject,
  beforeEachProviders
} from '@angular/core/testing';

import { TestComponentBuilder } from '@angular/compiler/testing';

import { AboutComponent } from './about.component';

describe('About Component', () => {

  beforeEachProviders(() => []);

  it('should log on ngOnInit', async(inject([TestComponentBuilder], (tcb: TestComponentBuilder) => {
    tcb.createAsync(AboutComponent).then((fixture) => {
      fixture.detectChanges();

      spyOn(console, 'log');
      expect(console.log).not.toHaveBeenCalled();

      fixture.componentInstance.ngOnInit();
      expect(console.log).toHaveBeenCalledWith('Hello About');
    });
  })));

});
