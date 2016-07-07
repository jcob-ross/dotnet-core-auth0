describe('App', function () {

  beforeEach(function () {
    browser.get('/');
  });

  it('should have a title', function () {
    expect(browser.getTitle()).toEqual('ASP.NET Core & Angular 2 App with Auth0');
  });

  it('should have <header>', function () {
    expect(element(by.css('my-app header')).isPresent()).toEqual(true);
  });

  it('should have <main>', function () {
    expect(element(by.css('my-app main')).isPresent()).toEqual(true);
  });

  it('should have a main title', function () {
    expect(element(by.css('main .title')).getText()).toEqual('Hello from Angular 2!');
  });

  it('should have <footer>', function () {
    expect(element(by.css('my-app footer')).getText()).toEqual('\u2661 Angular 2 ASP.NET core with Auth0');
  });

});
