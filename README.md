# Nx full-stack Angular + .NET workspace with OIDC

## Architecture overview

- Angular shop application

  - Angular 17
  - Full use of standalone components and the functional APIs
  - Reactive state management using [@ngrx/signals](https://www.npmjs.com/package/@ngrx/signals)
  - OAuth 2.0 authorization code flow + PKCE and refresh tokens using [angular-auth-oidc-client](https://www.npmjs.com/package/angular-auth-oidc-client)

- Shop web API

  - .NET 8
  - Restful HTTP controllers
  - SQLite database with a custom unit of work implementation on top of [Dapper](https://www.nuget.org/packages/Dapper)
  - Auto-generated OpenAPI documentation using [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore)
  - OAuth 2.0 bearer token authorization with introspection using [OpenIddict.AspNetCore](https://www.nuget.org/packages/OpenIddict.AspNetCore)
  - Job scheduling using [Quartz](https://www.nuget.org/packages/Quartz)

## Quick Start

Angular Shop application

- Run `npx nx serve shop` to start the Angular Shop application. Navigate to http://localhost:5201/.

Shop API

- Run `npx nx serve shop-api` to start the Shop Web API. Navigate to https://localhost:7101/.

Identity Server application

- Run `npx nx serve identity-server` to start the Identity Server application. Navigate to https://localhost:7001/.
- Run `npx nx build identity-server` to build the Identity Server application.
- Run `npx nx publish identity-server` to publish the Identity Server application into the `dist/publish/apps/identity-server` folder. The executable can also be run on the local machine using `./IdentityServer`.

All applications

- Run `npx nx run-many --target=serve --all` to start all applications at once.
