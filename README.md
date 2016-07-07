## `WIP` - Angular2 /w Webpack SPA (client) and ASP.NET Core as backend
(Auth0 & JWT for authentication/authorization)

[Is Angular 2 Ready Yet?](http://splintercode.github.io/is-angular-2-ready/) (tl;dr; No, but soon<sup>TM</sup>)

## Quick start
There is no 'quick' start.

## Dependencies

`Node > 5`</br>
`NPM > 3` 

#### /Installation

> ##Project is split into: </br>
> backend `(src/dotnet-core-auth0.Backend)` - backend server</br>
> frontend `(src/dotnet-core-auth0.Client)` - SPA client, contains client tests too (karma, protractor)</br>
> data layer `(src/dotnet-core-auth0.Data)` - EF & persistence etc.</br>

> backend tests `(test/dotnet-core-auth0.Backend.Test)` - xUnit tests</br>
> data layer tests `(test/dotnet-core-auth0.Data.Test)` - xUnit tests</br>
</br>

>(watch over node_modules folder with 50k+ files slows [thigs down](https://github.com/dotnet/cli/issues/3796))

> And because client and backend is separated, there is gulp task to copy client</br>
> to backend's `wwwroot`. That task is in `project.json` as a `prepublish` step.
> Backend runs on 8080 by default, client (dev server, karma, protractor) on 8081.
> Api calls from client are directed at 8080.


> So if you want to go and set up Auth0, make sure you allow those ports where needed.
  (I'm using SecretManager, so substitute that for appsettings.json, see example-secrets.json)

> And: </br>
>   
> * add your client info to `appsettings.json` (Domain, Client ID, Client secret)
> * set appropriate callback/logout/origins (Auth0 dashboard)
> * switch JWT signature algorithm to `RS256` (Auth0 dashboard - advanced)
> * copy signing certificate to `CertificateString` field in `appsettings.json`</br> 
>   (as <strong>single</strong> line <strong>without</strong> ---BEGIN CERT.. and  ...END CERT..---</br>
>    string -> bytes -> fed into X509Certificate' ctor)

```bash
git clone

# in client folder (src/dotnet-core-auth0.Client)
npm install # runs webdriver-update and typings install in postinstall script

# in backend folder (src/dotnet-core-auth0 etc.)
dotnet restore 
```

## Backend Gulp scripts
```bash
gulp clean # cleans /wwwroot
gulp copy-client # copies files from client's /dist folder to backend's /wwwroot

# it will scream and whine if those files are missing (see gulpfile.js)  
```

## Client NPM scripts

#### /Build

* dev server watch mode (port 8081): `npm run server` or `npm run start`
* single run (minified): `npm run build`
* single run (non-minified): `npm run build-dev`
* build files and watch: `npm run watch`


#### /Testing (coverage included)

* single run: `npm test`
* live mode: `npm run test-watch`
* e2e with protractor: `npm run e2e` (needs dev server running)


#### /Docs
* To generate docs using [TypeDoc](http://typedoc.io/) :
`npm run docs`

